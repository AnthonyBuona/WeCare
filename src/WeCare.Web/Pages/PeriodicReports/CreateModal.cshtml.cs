using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.PeriodicReports;

namespace WeCare.Web.Pages.PeriodicReports
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdatePeriodicReportDto PeriodicReport { get; set; }

        public List<SelectListItem> PatientList { get; set; }
        public List<SelectListItem> TherapistList { get; set; }

        private readonly IPeriodicReportAppService _periodicReportAppService;

        public CreateModalModel(IPeriodicReportAppService periodicReportAppService)
        {
            _periodicReportAppService = periodicReportAppService;
        }

        public async Task OnGetAsync()
        {
            PeriodicReport = new CreateUpdatePeriodicReportDto
            {
                Title = "Relatório Periódico de Evolução",
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                Status = PeriodicReportStatus.Draft,
                ResumoClinico = "",
                ObjetivosStatus = "[]",
                EngajamentoCasa = "",
                ProximosPassos = ""
            };

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
            await _periodicReportAppService.CreateAsync(PeriodicReport);
            return NoContent();
        }
    }
}
