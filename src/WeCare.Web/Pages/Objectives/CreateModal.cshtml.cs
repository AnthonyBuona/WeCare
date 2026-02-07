using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Objectives;

namespace WeCare.Web.Pages.Objectives
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateObjectiveDto Objective { get; set; }

        public List<SelectListItem> PatientList { get; set; }
        public List<SelectListItem> TherapistList { get; set; }

        private readonly IObjectiveAppService _objectiveAppService;

        public CreateModalModel(IObjectiveAppService objectiveAppService)
        {
            _objectiveAppService = objectiveAppService;
        }

        public async Task OnGetAsync()
        {
            Objective = new CreateUpdateObjectiveDto();
            Objective.StartDate = DateTime.Now; 

            var patientLookup = await _objectiveAppService.GetPatientLookupAsync();
            PatientList = patientLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();

            var therapistLookup = await _objectiveAppService.GetTherapistLookupAsync();
            TherapistList = therapistLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _objectiveAppService.CreateAsync(Objective);
            return NoContent();
        }
    }
}
