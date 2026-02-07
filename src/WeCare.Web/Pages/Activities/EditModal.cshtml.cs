using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Activities;

namespace WeCare.Web.Pages.Activities
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateActivityDto Activity { get; set; }

        public List<SelectListItem> TrainingList { get; set; }

        private readonly IActivityAppService _activityAppService;

        public EditModalModel(IActivityAppService activityAppService)
        {
            _activityAppService = activityAppService;
        }

        public async Task OnGetAsync()
        {
            var activityDto = await _activityAppService.GetAsync(Id);
            Activity = ObjectMapper.Map<ActivityDto, CreateUpdateActivityDto>(activityDto);

            var trainingLookup = await _activityAppService.GetTrainingLookupAsync();
            TrainingList = trainingLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _activityAppService.UpdateAsync(Id, Activity);
            return NoContent();
        }
    }
}
