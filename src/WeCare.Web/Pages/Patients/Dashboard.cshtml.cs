using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Dashboards;

namespace WeCare.Web.Pages.Patients
{
    public class DashboardModel : WeCarePageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public PatientDashboardDto DashboardData { get; set; }

        private readonly IDashboardAppService _dashboardAppService;

        public DashboardModel(IDashboardAppService dashboardAppService)
        {
            _dashboardAppService = dashboardAppService;
        }

        public async Task OnGetAsync()
        {
            DashboardData = await _dashboardAppService.GetPatientDashboardAsync(Id);
        }
    }
}
