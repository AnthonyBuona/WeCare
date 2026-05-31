#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.Gamification
{
    public class UserGamifiedProfile : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid ParentUserId { get; set; }

        public int CurrentLevel { get; set; }

        public int TotalXp { get; set; }

        public int ActiveStreak { get; set; }

        public string UnlockedBadgesJson { get; set; }

        protected UserGamifiedProfile()
        {
        }

        public UserGamifiedProfile(
            Guid id,
            Guid parentUserId,
            int currentLevel,
            int totalXp,
            int activeStreak,
            string unlockedBadgesJson,
            Guid? tenantId = null)
            : base(id)
        {
            ParentUserId = parentUserId;
            CurrentLevel = currentLevel;
            TotalXp = totalXp;
            ActiveStreak = activeStreak;
            UnlockedBadgesJson = unlockedBadgesJson;
            TenantId = tenantId;
        }
    }
}
