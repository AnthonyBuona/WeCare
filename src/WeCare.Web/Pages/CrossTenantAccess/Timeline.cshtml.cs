using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using WeCare.Attendances;
using WeCare.Consultations;
using WeCare.CrossTenantAccess;
using WeCare.Patients;
using WeCare.PeriodicReports;
using WeCare.Therapists;

namespace WeCare.Web.Pages.CrossTenantAccess
{
    public class TimelineModel : WeCarePageModel
    {
        private readonly ICrossTenantAccessAppService _crossTenantAccessAppService;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Consultation, Guid> _consultationRepository;
        private readonly IRepository<Attendance, Guid> _attendanceRepository;
        private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IDataFilter _dataFilter;

        public TimelineModel(
            ICrossTenantAccessAppService crossTenantAccessAppService,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Attendance, Guid> attendanceRepository,
            IRepository<PeriodicReport, Guid> periodicReportRepository,
            IRepository<Therapist, Guid> therapistRepository,
            ITenantRepository tenantRepository,
            IDataFilter dataFilter)
        {
            _crossTenantAccessAppService = crossTenantAccessAppService;
            _patientRepository = patientRepository;
            _consultationRepository = consultationRepository;
            _attendanceRepository = attendanceRepository;
            _periodicReportRepository = periodicReportRepository;
            _therapistRepository = therapistRepository;
            _tenantRepository = tenantRepository;
            _dataFilter = dataFilter;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public bool IsAccessGranted { get; set; }
        public string ErrorMessage { get; set; }

        public PatientDto Patient { get; set; }
        public string OriginClinicName { get; set; }

        public List<TimelineItemViewModel> TimelineItems { get; set; } = new List<TimelineItemViewModel>();

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                return;
            }

            await LoadTimelineAsync();
        }

        public async Task<IActionResult> OnPostSubmitTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                ErrorMessage = "O token de acesso não pode ser vazio.";
                return Page();
            }

            return RedirectToPage(new { Token });
        }

        private async Task LoadTimelineAsync()
        {
            try
            {
                // Verify consent through the app service to perform formal checks and audit logging
                var consentDto = await _crossTenantAccessAppService.VerifyConsentAsync(new VerifyConsentTokenDto
                {
                    RawToken = Token
                });

                if (consentDto == null || consentDto.IsRevoked || consentDto.ExpirationDate < Clock.Now)
                {
                    ErrorMessage = "Acesso negado. O token de consentimento é inválido, revogado ou expirou.";
                    return;
                }

                IsAccessGranted = true;

                // Load source clinic/tenant name
                var sourceTenant = await _tenantRepository.FindAsync(consentDto.SourceTenantId);
                OriginClinicName = sourceTenant?.Name ?? "Clínica Crescer (Origem)";

                // Switch to the source tenant database context to retrieve patient medical records safely
                using (CurrentTenant.Change(consentDto.SourceTenantId))
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    var patient = await _patientRepository.FirstOrDefaultAsync(p => p.Id == consentDto.PatientId);
                    if (patient == null)
                    {
                        ErrorMessage = "Paciente associado a este token não foi encontrado.";
                        IsAccessGranted = false;
                        return;
                    }

                    Patient = new PatientDto
                    {
                        Id = patient.Id,
                        Name = patient.Name,
                        Diag = patient.Diag ?? "Sem diagnóstico informado",
                        BirthDate = patient.BirthDate
                    };

                    // Retrieve details
                    var consultations = await _consultationRepository.GetListAsync(c => c.PatientId == patient.Id);
                    var attendances = await _attendanceRepository.GetListAsync(a => a.PatientId == patient.Id);
                    var periodicReports = await _periodicReportRepository.GetListAsync(pr => pr.PatientId == patient.Id);
                    var therapists = await _therapistRepository.GetListAsync();

                    // Map to a unified chronologic timeline
                    TimelineItems = new List<TimelineItemViewModel>();

                    // 1. Consultations
                    foreach (var c in consultations)
                    {
                        var therapist = therapists.FirstOrDefault(t => t.Id == c.TherapistId);
                        string therapistName = therapist?.Name ?? "Dra. Camila Terapeuta";

                        TimelineItems.Add(new TimelineItemViewModel
                        {
                            Date = c.DateTime,
                            Title = $"Consulta - {c.Specialty}",
                            Type = "Consultation",
                            IconClass = "fa fa-user-md bg-info text-white",
                            BadgeClass = "bg-light-info text-info",
                            Description = c.Description,
                            AdditionalInfo = $"Treinamento: {c.MainTraining} ({c.Duration}) | Terapeuta: {therapistName}"
                        });
                    }

                    // 2. Attendances
                    foreach (var a in attendances)
                    {
                        string statusLabel = a.Status == AttendanceStatus.Present ? "Presente" : "Falta";
                        string statusBadge = a.Status == AttendanceStatus.Present ? "bg-success" : "bg-danger";

                        TimelineItems.Add(new TimelineItemViewModel
                        {
                            Date = a.SessionDate,
                            Title = $"Registro de Presença: {statusLabel}",
                            Type = "Attendance",
                            IconClass = $"fa fa-calendar-check-o {statusBadge} text-white",
                            BadgeClass = a.Status == AttendanceStatus.Present ? "bg-light-success text-success" : "bg-light-danger text-danger",
                            Description = a.Notes,
                            AdditionalInfo = $"Status: {statusLabel}"
                        });
                    }

                    // 3. Periodic Reports
                    foreach (var pr in periodicReports)
                    {
                        TimelineItems.Add(new TimelineItemViewModel
                        {
                            Date = pr.EndDate,
                            Title = $"{pr.Title} - {pr.Status}",
                            Type = "Report",
                            IconClass = "fa fa-file-text-o bg-warning text-dark",
                            BadgeClass = "bg-light-warning text-warning",
                            Description = pr.ResumoClinico,
                            AdditionalInfo = $"Próximos Passos: {pr.ProximosPassos} | Engajamento em Casa: {pr.EngajamentoCasa}"
                        });
                    }

                    // Sort chronologically (newest first)
                    TimelineItems = TimelineItems.OrderByDescending(x => x.Date).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                IsAccessGranted = false;
            }
        }
    }

    public class PatientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Diag { get; set; }
        public DateTime BirthDate { get; set; }

        public int Age
        {
            get
            {
                var age = DateTime.Today.Year - BirthDate.Year;
                if (BirthDate.Date > DateTime.Today.AddYears(-age)) age--;
                return age;
            }
        }
    }

    public class TimelineItemViewModel
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public string BadgeClass { get; set; }
        public string Description { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
