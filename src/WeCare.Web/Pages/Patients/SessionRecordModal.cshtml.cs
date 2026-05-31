using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Domain.Repositories;
using WeCare.Attendances;
using WeCare.Consultations;
using WeCare.Application.Contracts.Consultations;
using WeCare.Objectives;
using WeCare.Patients;
using WeCare.Therapists;

namespace WeCare.Web.Pages.Patients
{
    public class SessionRecordModalModel : WeCarePageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PatientId { get; set; }

        [BindProperty]
        public SessionWizardInput Input { get; set; }

        public string PatientName { get; set; }
        public List<SelectListItem> TherapistList { get; set; }
        public List<ObjectiveDto> ActiveObjectives { get; set; }

        private readonly IPatientAppService _patientAppService;
        private readonly IAttendanceAppService _attendanceAppService;
        private readonly IObjectiveAppService _objectiveAppService;
        private readonly IConsultationAppService _consultationAppService;
        private readonly ITherapistAppService _therapistAppService;
        private readonly IRepository<Objective, Guid> _objectiveRepository;

        public SessionRecordModalModel(
            IPatientAppService patientAppService,
            IAttendanceAppService attendanceAppService,
            IObjectiveAppService objectiveAppService,
            IConsultationAppService consultationAppService,
            ITherapistAppService therapistAppService,
            IRepository<Objective, Guid> objectiveRepository)
        {
            _patientAppService = patientAppService;
            _attendanceAppService = attendanceAppService;
            _objectiveAppService = objectiveAppService;
            _consultationAppService = consultationAppService;
            _therapistAppService = therapistAppService;
            _objectiveRepository = objectiveRepository;
        }

        public async Task OnGetAsync()
        {
            var patient = await _patientAppService.GetAsync(PatientId);
            PatientName = patient.Name;

            // Load active objectives directly from repository to avoid DTO mapping issues
            var objectives = await _objectiveRepository.GetListAsync(x => x.PatientId == PatientId);
            ActiveObjectives = objectives.Select(x => ObjectMapper.Map<Objective, ObjectiveDto>(x)).ToList();

            // Load therapists
            var therapistLookup = await _therapistAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 100 });
            TherapistList = therapistLookup.Items
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList();

            Input = new SessionWizardInput
            {
                SessionDate = DateTime.Now,
                NextSessionDate = DateTime.Now.AddDays(7).Date.AddHours(14), // Default to next week at 14:00
                Status = AttendanceStatus.Present
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Create Attendance Record
            var attendanceDto = new CreateUpdateAttendanceDto
            {
                PatientId = PatientId,
                SessionDate = Input.SessionDate,
                Status = Input.Status,
                Notes = Input.SessionNotes ?? "Sessão realizada via painel rápido."
            };
            await _attendanceAppService.CreateAsync(attendanceDto);

            // 2. Update Objectives Progress if selected
            if (Input.ObjectiveProgress != null && Input.ObjectiveProgress.Any())
            {
                foreach (var progress in Input.ObjectiveProgress)
                {
                    if (Guid.TryParse(progress.Key, out var objectiveId) && !string.IsNullOrEmpty(progress.Value))
                    {
                        var obj = await _objectiveAppService.GetAsync(objectiveId);
                        if (obj != null)
                        {
                            var updateDto = new CreateUpdateObjectiveDto
                            {
                                PatientId = obj.PatientId,
                                TherapistId = obj.TherapistId,
                                Name = obj.Name,
                                Status = progress.Value,
                                StartDate = obj.StartDate,
                                EndDate = obj.EndDate
                            };
                            await _objectiveAppService.UpdateAsync(objectiveId, updateDto);
                        }
                    }
                }
            }

            // 3. Schedule next session if enabled
            if (Input.ScheduleNextSession && Input.NextSessionTherapistId.HasValue)
            {
                var patient = await _patientAppService.GetAsync(PatientId);
                
                // Fetch first treatment of the patient if any
                Guid defaultTreatmentId = Guid.Empty;
                try 
                {
                    var consults = await _consultationAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 5 });
                    var match = consults.Items.FirstOrDefault(c => c.PatientId == PatientId);
                    if (match != null)
                    {
                        defaultTreatmentId = match.TratamentoId;
                    }
                }
                catch { }

                if (defaultTreatmentId == Guid.Empty)
                {
                    // Fallback to avoid crashes
                    defaultTreatmentId = Guid.NewGuid();
                }

                var consultationDto = new CreateUpdateConsultationDto
                {
                    PatientId = PatientId,
                    TherapistId = Input.NextSessionTherapistId.Value,
                    TratamentoId = defaultTreatmentId,
                    DateTime = Input.NextSessionDate,
                    Specialty = "Terapia Ocupacional",
                    Description = "Sessão agendada automaticamente via assistente de evolução.",
                    Status = ConsultationStatus.Agendada
                };
                await _consultationAppService.CreateAsync(consultationDto);
            }

            return NoContent();
        }
    }

    public class SessionWizardInput
    {
        public DateTime SessionDate { get; set; }
        public AttendanceStatus Status { get; set; }
        public string SessionNotes { get; set; }

        public Dictionary<string, string> ObjectiveProgress { get; set; } = new();

        public bool ScheduleNextSession { get; set; }
        public DateTime NextSessionDate { get; set; }
        public Guid? NextSessionTherapistId { get; set; }
    }
}
