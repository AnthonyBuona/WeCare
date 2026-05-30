using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WeCare.Clinics;
using WeCare.Dashboards;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Consultations;
using WeCare.PeriodicReports;

namespace WeCare.Web.Pages;

public class IndexModel : WeCarePageModel
{
    public WeCareDashboardHeaderStatsDto? Stats { get; set; }
    public ClinicSettingsDto? ClinicInfo { get; set; }
    
    public bool IsResponsible { get; set; }
    public Responsible? ParentProfile { get; set; }
    public List<Patient> MyPatients { get; set; } = new();
    public List<Consultation> RecentConsultations { get; set; } = new();
    public List<PeriodicReport> MyReports { get; set; } = new();
    public string ClientIp { get; set; } = "127.0.0.1";

    private readonly IDashboardAppService _dashboardAppService;
    private readonly IClinicAppService _clinicAppService;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Responsible, Guid> _responsibleRepository;
    private readonly IRepository<Consultation, Guid> _consultationRepository;
    private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;

    public IndexModel(
        IDashboardAppService dashboardAppService, 
        IClinicAppService clinicAppService,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Responsible, Guid> responsibleRepository,
        IRepository<Consultation, Guid> consultationRepository,
        IRepository<PeriodicReport, Guid> periodicReportRepository)
    {
        _dashboardAppService = dashboardAppService;
        _clinicAppService = clinicAppService;
        _patientRepository = patientRepository;
        _responsibleRepository = responsibleRepository;
        _consultationRepository = consultationRepository;
        _periodicReportRepository = periodicReportRepository;
    }

    public async Task OnGetAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            Stats = await _dashboardAppService.GetStatsAsync();

            try
            {
                ClinicInfo = await _clinicAppService.GetCurrentClinicSettingsAsync();
            }
            catch (Exception)
            {
                // Clinic info is non-critical
            }

            // Check if user is in "Responsible" role
            if (CurrentUser.Roles.Contains("Responsible") || CurrentUser.Roles.Contains("responsible"))
            {
                IsResponsible = true;
                ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                if (ClientIp == "::1")
                {
                    ClientIp = "127.0.0.1";
                }
                
                ParentProfile = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
                if (ParentProfile != null)
                {
                    MyPatients = await _patientRepository.GetListAsync(p => p.PrincipalResponsibleId == ParentProfile.Id);
                    if (MyPatients.Any())
                    {
                        var patientIds = MyPatients.Select(p => p.Id).ToList();
                        
                        // Query consultations for my patients
                        var consultations = await _consultationRepository.GetListAsync(c => patientIds.Contains(c.PatientId));
                        RecentConsultations = consultations.OrderByDescending(c => c.DateTime).Take(5).ToList();

                        // Query published reports for my patients
                        var reports = await _periodicReportRepository.GetListAsync(r => patientIds.Contains(r.PatientId) && r.Status != PeriodicReportStatus.Draft);
                        MyReports = reports.OrderByDescending(r => r.CreationTime).ToList();
                    }
                }
            }
        }
    }
}
