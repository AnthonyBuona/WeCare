using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.PeriodicReports;

namespace WeCare.Web.Pages.PeriodicReports
{
    public class ViewModalModel : WeCarePageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public PeriodicReportDto PeriodicReport { get; set; }

        private readonly IPeriodicReportAppService _periodicReportAppService;

        public ViewModalModel(IPeriodicReportAppService periodicReportAppService)
        {
            _periodicReportAppService = periodicReportAppService;
        }

        public async Task OnGetAsync()
        {
            PeriodicReport = await _periodicReportAppService.GetAsync(Id);
        }
    }
}
