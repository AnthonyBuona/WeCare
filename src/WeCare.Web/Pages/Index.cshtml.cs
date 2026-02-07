using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeCare.Dashboards;

namespace WeCare.Web.Pages;

public class IndexModel : WeCarePageModel
{
    public WeCareDashboardHeaderStatsDto? Stats { get; set; }

    private readonly IDashboardAppService _dashboardAppService;

    public IndexModel(IDashboardAppService dashboardAppService)
    {
        _dashboardAppService = dashboardAppService;
    }

    public async Task OnGetAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            Stats = await _dashboardAppService.GetStatsAsync();
        }
    }
}
