using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Permissions;

namespace WeCare.Gamification
{
    public class GamificationAppService : ApplicationService, IGamificationAppService
    {
        private readonly IRepository<CaregiverQuest, Guid> _questRepository;
        private readonly IRepository<QuestExecutionLog, Guid> _logRepository;
        private readonly IRepository<UserGamifiedProfile, Guid> _profileRepository;
        private readonly IDistributedCache _cache;

        private const string StreakCachePrefix = "UserStreakCache:";

        public GamificationAppService(
            IRepository<CaregiverQuest, Guid> questRepository,
            IRepository<QuestExecutionLog, Guid> logRepository,
            IRepository<UserGamifiedProfile, Guid> profileRepository,
            IDistributedCache cache)
        {
            _questRepository = questRepository;
            _logRepository = logRepository;
            _profileRepository = profileRepository;
            _cache = cache;
        }

        public async Task<CaregiverQuestDto> CreateQuestAsync(Guid patientId, string title, string instructions, string videoTutorialUrl, int xpReward)
        {
            await CheckPolicyAsync(WeCarePermissions.Gamification.CreateQuest);

            var quest = new CaregiverQuest(
                GuidGenerator.Create(),
                patientId,
                title,
                instructions,
                videoTutorialUrl,
                xpReward,
                CurrentTenant.Id
            );

            await _questRepository.InsertAsync(quest);
            return ObjectMapper.Map<CaregiverQuest, CaregiverQuestDto>(quest);
        }

        public async Task<List<CaregiverQuestDto>> GetQuestsByPatientAsync(Guid patientId)
        {
            var list = await _questRepository.GetListAsync(x => x.PatientId == patientId);
            return ObjectMapper.Map<List<CaregiverQuest>, List<CaregiverQuestDto>>(list);
        }

        public async Task<QuestExecutionLogDto> ExecuteQuestAsync(CreateQuestExecutionLogDto input)
        {
            await CheckPolicyAsync(WeCarePermissions.Gamification.ExecuteQuest);

            var quest = await _questRepository.FirstOrDefaultAsync(x => x.Id == input.QuestId);
            if (quest == null)
            {
                throw new UserFriendlyException("A Quest selecionada não existe.");
            }

            var userId = CurrentUser.Id ?? Guid.Empty;
            if (userId == Guid.Empty)
            {
                throw new UserFriendlyException("Usuário cuidador precisa estar logado para realizar quests.");
            }

            // Create quest log
            var log = new QuestExecutionLog(
                GuidGenerator.Create(),
                input.QuestId,
                Clock.Now,
                input.EnjoymentScore,
                input.CaregiverNotes,
                CurrentTenant.Id
            );

            await _logRepository.InsertAsync(log);

            // Reward calculation
            var profile = await _profileRepository.FirstOrDefaultAsync(x => x.ParentUserId == userId);
            if (profile == null)
            {
                // Create user gamified profile
                profile = new UserGamifiedProfile(
                    GuidGenerator.Create(),
                    userId,
                    currentLevel: 1,
                    totalXp: 0,
                    activeStreak: 0,
                    unlockedBadgesJson: "[]",
                    tenantId: CurrentTenant.Id
                );
                await _profileRepository.InsertAsync(profile);
            }

            // Update XP
            profile.TotalXp += quest.XpReward;

            // Simple RPG level progression: Level = (XP / 100) + 1
            int oldLevel = profile.CurrentLevel;
            int newLevel = (profile.TotalXp / 100) + 1;
            profile.CurrentLevel = newLevel;

            // Badges parsing & unlocking
            var badges = ParseBadges(profile.UnlockedBadgesJson);
            bool badgesChanged = false;

            if (newLevel > oldLevel)
            {
                // Level Up Badge
                string levelUpBadge = $"Nível {newLevel} Conquistado";
                if (!badges.Contains(levelUpBadge))
                {
                    badges.Add(levelUpBadge);
                    badgesChanged = true;
                }
            }

            // Custom milestone badges
            if (profile.TotalXp >= 500 && !badges.Contains("Bronze Caregiver"))
            {
                badges.Add("Bronze Caregiver");
                badgesChanged = true;
            }
            if (profile.TotalXp >= 1500 && !badges.Contains("Silver Caregiver"))
            {
                badges.Add("Silver Caregiver");
                badgesChanged = true;
            }
            if (profile.TotalXp >= 3000 && !badges.Contains("Gold Caregiver"))
            {
                badges.Add("Gold Caregiver");
                badgesChanged = true;
            }

            if (badgesChanged)
            {
                profile.UnlockedBadgesJson = "[" + string.Join(",", badges.Select(b => $"\"{b}\"")) + "]";
            }

            // Sync daily caregiver streak in real time
            int newStreak = await IncrementStreakAsync(userId);
            profile.ActiveStreak = newStreak;

            await _profileRepository.UpdateAsync(profile);

            return ObjectMapper.Map<QuestExecutionLog, QuestExecutionLogDto>(log);
        }

        public async Task<UserGamifiedProfileDto> GetProfileAsync(Guid parentUserId)
        {
            await CheckPolicyAsync(WeCarePermissions.Gamification.ViewProfile);

            var profile = await _profileRepository.FirstOrDefaultAsync(x => x.ParentUserId == parentUserId);
            if (profile == null)
            {
                // Return default empty profile DTO
                return new UserGamifiedProfileDto
                {
                    ParentUserId = parentUserId,
                    CurrentLevel = 1,
                    TotalXp = 0,
                    ActiveStreak = await GetActiveStreakAsync(parentUserId),
                    UnlockedBadgesJson = "[]"
                };
            }

            // Always sync active streak from Redis real-time cache
            profile.ActiveStreak = await GetActiveStreakAsync(parentUserId);
            return ObjectMapper.Map<UserGamifiedProfile, UserGamifiedProfileDto>(profile);
        }

        public async Task<int> GetActiveStreakAsync(Guid parentUserId)
        {
            string cacheKey = StreakCachePrefix + parentUserId.ToString("N");
            string cachedValue = null;

            try
            {
                cachedValue = await _cache.GetStringAsync(cacheKey);
            }
            catch
            {
                // Fallback gracefully on cache error (e.g. Redis disconnected)
            }

            if (cachedValue != null && int.TryParse(cachedValue, out int streak))
            {
                return streak;
            }

            // Fallback: Query database
            var profile = await _profileRepository.FirstOrDefaultAsync(x => x.ParentUserId == parentUserId);
            int dbStreak = profile?.ActiveStreak ?? 0;

            // Sync back to Redis cache (expires in 24 hours)
            try
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    dbStreak.ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    }
                );
            }
            catch
            {
                // Fallback gracefully
            }

            return dbStreak;
        }

        public async Task<int> IncrementStreakAsync(Guid parentUserId)
        {
            int currentStreak = await GetActiveStreakAsync(parentUserId);
            int newStreak = currentStreak + 1;

            string cacheKey = StreakCachePrefix + parentUserId.ToString("N");

            // Write to Redis cache
            try
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    newStreak.ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    }
                );
            }
            catch
            {
                // Fallback gracefully
            }

            // Sync to Database
            var profile = await _profileRepository.FirstOrDefaultAsync(x => x.ParentUserId == parentUserId);
            if (profile != null)
            {
                profile.ActiveStreak = newStreak;
                await _profileRepository.UpdateAsync(profile);
            }
            else
            {
                profile = new UserGamifiedProfile(
                    GuidGenerator.Create(),
                    parentUserId,
                    currentLevel: 1,
                    totalXp: 0,
                    activeStreak: newStreak,
                    unlockedBadgesJson: "[]",
                    tenantId: CurrentTenant.Id
                );
                await _profileRepository.InsertAsync(profile);
            }

            return newStreak;
        }

        #region Private Helpers

        private List<string> ParseBadges(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "[]")
            {
                return new List<string>();
            }

            // Simple parsing to avoid complex external library dependencies
            return json
                .Replace("[", "")
                .Replace("]", "")
                .Split(',')
                .Select(s => s.Trim().Trim('"'))
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        #endregion
    }
}
