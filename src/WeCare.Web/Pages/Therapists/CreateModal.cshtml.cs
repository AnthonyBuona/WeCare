using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Therapists;

namespace WeCare.Web.Pages.Therapists
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateTherapistDto Therapist { get; set; }

        private readonly ITherapistAppService _therapistAppService;

        public CreateModalModel(ITherapistAppService therapistAppService)
        {
            _therapistAppService = therapistAppService;
            Therapist = new CreateUpdateTherapistDto();
        }

        public void OnGet()
        {
            // Nothing to do
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _therapistAppService.CreateAsync(Therapist);
            return NoContent();
        }
    }
}