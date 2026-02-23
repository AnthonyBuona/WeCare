using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Application.Contracts.Consultations;
using WeCare.Application.Contracts.PerformedTrainings;
using WeCare.Consultations;
using WeCare.Domain.Shared.PerformedTrainings;
using WeCare.Objectives;
using WeCare.Patients;
using WeCare.Therapists;

namespace WeCare.Web.Pages.Session
{
    public class RecordModel : WeCarePageModel
    {
        // Data loaded for the view
        public ConsultationDto Consultation { get; set; }
        public string PatientName { get; set; }
        public string TherapistName { get; set; }
        public SelectList ObjectiveLookup { get; set; }
        public int AppointmentDurationMinutes { get; set; } = 30;

        [BindProperty]
        public SessionFormModel SessionData { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly IObjectiveAppService _objectiveAppService;
        private readonly IPatientAppService _patientAppService;
        private readonly ITherapistAppService _therapistAppService;
        private readonly WeCare.Clinics.IClinicAppService _clinicAppService;

        public RecordModel(
            IConsultationAppService consultationAppService,
            IObjectiveAppService objectiveAppService,
            IPatientAppService patientAppService,
            ITherapistAppService therapistAppService,
            WeCare.Clinics.IClinicAppService clinicAppService)
        {
            _consultationAppService = consultationAppService;
            _objectiveAppService = objectiveAppService;
            _patientAppService = patientAppService;
            _therapistAppService = therapistAppService;
            _clinicAppService = clinicAppService;
        }

        public async Task<IActionResult> OnGetAsync(Guid consultationId)
        {
            Consultation = await _consultationAppService.GetAsync(consultationId);

            if (Consultation == null)
            {
                return NotFound();
            }

            if (Consultation.Status != ConsultationStatus.Agendada)
            {
                return RedirectToPage("/Calendar/Index");
            }

            // Load names
            var patient = await _patientAppService.GetAsync(Consultation.PatientId);
            PatientName = patient.Name;

            var therapist = await _therapistAppService.GetAsync(Consultation.TherapistId);
            TherapistName = therapist.Name;

            // Load objectives for this patient
            var objectiveLookupResult = await _objectiveAppService.GetObjectiveLookupAsync(Consultation.PatientId);
            ObjectiveLookup = new SelectList(objectiveLookupResult.Items, "Id", "DisplayName");

            // Get appointment duration from clinic settings
            try
            {
                var settings = await _clinicAppService.GetCurrentClinicSettingsAsync();
                if (settings?.AppointmentDurationMinutes > 0)
                {
                    AppointmentDurationMinutes = settings.AppointmentDurationMinutes;
                }
            }
            catch { }

            // Pre-fill form
            SessionData = new SessionFormModel
            {
                ConsultationId = Consultation.Id,
                Description = "",
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dto = new CreateUpdateConsultationDto
            {
                PatientId = Guid.Empty, // Not used in CompleteSession
                TherapistId = Guid.Empty, // Not used in CompleteSession
                DateTime = DateTime.Now,
                Description = SessionData.Description ?? "Sessão realizada",
                MainTraining = SessionData.MainTraining,
                Duration = SessionData.Duration,
                ObjectiveId = SessionData.ObjectiveId,
                Status = ConsultationStatus.Realizada,
                PerformedTrainings = SessionData.PerformedTrainings?.Select(pt => new CreateUpdatePerformedTrainingDto
                {
                    TrainingId = pt.TrainingId,
                    HelpNeeded = Enum.TryParse<HelpNeededType>(pt.HelpNeeded, out var helpType)
                        ? helpType : HelpNeededType.I,
                    TotalAttempts = pt.TotalAttempts,
                    SuccessfulAttempts = pt.SuccessfulAttempts
                }).ToList() ?? new List<CreateUpdatePerformedTrainingDto>()
            };

            await _consultationAppService.CompleteSessionAsync(SessionData.ConsultationId, dto);

            return RedirectToPage("/Calendar/Index");
        }
    }

    public class SessionFormModel
    {
        [HiddenInput]
        public Guid ConsultationId { get; set; }

        public Guid? ObjectiveId { get; set; }

        [Display(Name = "Treino principal")]
        public string MainTraining { get; set; }

        [Display(Name = "Duração")]
        public string Duration { get; set; }

        [Display(Name = "Notas da sessão")]
        public string Description { get; set; }

        public List<SessionTrainingItem> PerformedTrainings { get; set; } = new();
    }

    public class SessionTrainingItem
    {
        public Guid TrainingId { get; set; }
        public string HelpNeeded { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
    }
}
