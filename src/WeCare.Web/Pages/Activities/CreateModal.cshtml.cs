using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Activities;

namespace WeCare.Web.Pages.Activities
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateActivityDto Activity { get; set; }

        public List<SelectListItem> TrainingList { get; set; }

        private readonly IActivityAppService _activityAppService;

        public CreateModalModel(IActivityAppService activityAppService)
        {
            _activityAppService = activityAppService;
        }

        public async Task OnGetAsync()
        {
            Activity = new CreateUpdateActivityDto();

            var trainingLookup = await _activityAppService.GetTrainingLookupAsync();
            TrainingList = trainingLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _activityAppService.CreateAsync(Activity);
            return NoContent();
        }
    }
}
