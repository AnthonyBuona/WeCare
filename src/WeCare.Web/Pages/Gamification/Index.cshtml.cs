using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using WeCare.Gamification;
using WeCare.Patients;
using WeCare.Responsibles;

namespace WeCare.Web.Pages.Gamification
{
    public class IndexModel : WeCarePageModel
    {
        private readonly IGamificationAppService _gamificationAppService;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;

        public IndexModel(
            IGamificationAppService gamificationAppService,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Responsible, Guid> responsibleRepository)
        {
            _gamificationAppService = gamificationAppService;
            _patientRepository = patientRepository;
            _responsibleRepository = responsibleRepository;
        }

        public UserGamifiedProfileDto Profile { get; set; }
        public List<CaregiverQuestDto> Quests { get; set; } = new List<CaregiverQuestDto>();
        public List<string> Badges { get; set; } = new List<string>();

        public Guid PatientId { get; set; }
        public string PatientName { get; set; }

        public int XpInCurrentLevel { get; set; }
        public int XpProgressPercentage { get; set; }

        [BindProperty]
        public Guid SelectedQuestId { get; set; }

        [BindProperty]
        public int EnjoymentScore { get; set; } = 5;

        [BindProperty]
        public string CaregiverNotes { get; set; }

        [TempData]
        public bool ShowConfetti { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostExecuteQuestAsync()
        {
            if (SelectedQuestId == Guid.Empty)
            {
                Alerts.Danger("Quest inválida.");
                return RedirectToPage();
            }

            try
            {
                var log = await _gamificationAppService.ExecuteQuestAsync(new CreateQuestExecutionLogDto
                {
                    QuestId = SelectedQuestId,
                    EnjoymentScore = EnjoymentScore,
                    CaregiverNotes = CaregiverNotes
                });

                ShowConfetti = true;
                Alerts.Success("Quest concluída! Você ganhou XP e Lucas se engajou muito!");
            }
            catch (Exception ex)
            {
                Alerts.Danger($"Falha ao registrar conclusão da quest: {ex.Message}");
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            Guid userId = CurrentUser.Id ?? Guid.Empty;

            // Load profile
            Profile = await _gamificationAppService.GetProfileAsync(userId);

            // Compute XP progression
            XpInCurrentLevel = Profile.TotalXp % 100;
            XpProgressPercentage = XpInCurrentLevel; // Since next level requires 100 XP, % is exactly XpInCurrentLevel

            // Parse Badges
            Badges = ParseBadges(Profile.UnlockedBadgesJson);

            // Fetch patient for this caregiver
            var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == userId);
            Patient patientEntity = null;
            if (responsible != null)
            {
                patientEntity = await _patientRepository.FirstOrDefaultAsync(x => x.PrincipalResponsibleId == responsible.Id);
            }

            if (patientEntity == null)
            {
                // Fallback: use first patient in the database
                patientEntity = await _patientRepository.FirstOrDefaultAsync();
            }

            if (patientEntity != null)
            {
                PatientId = patientEntity.Id;
                PatientName = patientEntity.Name;
                Quests = await _gamificationAppService.GetQuestsByPatientAsync(PatientId);
            }
        }

        private List<string> ParseBadges(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "[]")
            {
                return new List<string>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return json
                    .Replace("[", "")
                    .Replace("]", "")
                    .Split(',')
                    .Select(s => s.Trim().Trim('"'))
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }
        }
    }
}
