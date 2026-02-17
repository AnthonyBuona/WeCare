using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeCare.Clinics;
using WeCare.Dashboards;

namespace WeCare.Web.Pages;

public class IndexModel : WeCarePageModel
{
    public WeCareDashboardHeaderStatsDto? Stats { get; set; }
    public ClinicSettingsDto? ClinicInfo { get; set; }

    private readonly IDashboardAppService _dashboardAppService;
    private readonly IClinicAppService _clinicAppService;

    public IndexModel(IDashboardAppService dashboardAppService, IClinicAppService clinicAppService)
    {
        _dashboardAppService = dashboardAppService;
        _clinicAppService = clinicAppService;
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
        }
    }
}
