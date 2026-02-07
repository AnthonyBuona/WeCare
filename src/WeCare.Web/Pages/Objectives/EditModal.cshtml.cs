using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Objectives;

namespace WeCare.Web.Pages.Objectives
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateObjectiveDto Objective { get; set; }

        public List<SelectListItem> PatientList { get; set; }
        public List<SelectListItem> TherapistList { get; set; }

        private readonly IObjectiveAppService _objectiveAppService;

        public EditModalModel(IObjectiveAppService objectiveAppService)
        {
            _objectiveAppService = objectiveAppService;
        }

        public async Task OnGetAsync()
        {
            var objectiveDto = await _objectiveAppService.GetAsync(Id);
            Objective = ObjectMapper.Map<ObjectiveDto, CreateUpdateObjectiveDto>(objectiveDto);

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
            await _objectiveAppService.UpdateAsync(Id, Objective);
            return NoContent();
        }
    }
}
