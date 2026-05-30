using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.PeriodicReports;

namespace WeCare.Web.Pages.PeriodicReports
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdatePeriodicReportDto PeriodicReport { get; set; }

        public List<SelectListItem> PatientList { get; set; }
        public List<SelectListItem> TherapistList { get; set; }

        private readonly IPeriodicReportAppService _periodicReportAppService;

        public EditModalModel(IPeriodicReportAppService periodicReportAppService)
        {
            _periodicReportAppService = periodicReportAppService;
        }

        public async Task OnGetAsync()
        {
            var reportDto = await _periodicReportAppService.GetAsync(Id);
            PeriodicReport = ObjectMapper.Map<PeriodicReportDto, CreateUpdatePeriodicReportDto>(reportDto);

            var patientLookup = await _periodicReportAppService.GetPatientLookupAsync();
            PatientList = patientLookup
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();

            var therapistLookup = await _periodicReportAppService.GetTherapistLookupAsync();
            TherapistList = therapistLookup
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _periodicReportAppService.UpdateAsync(Id, PeriodicReport);
            return NoContent();
        }
    }
}
