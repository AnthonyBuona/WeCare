using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Therapists;

namespace WeCare.Web.Pages.Therapists
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateTherapistDto Therapist { get; set; }

        private readonly ITherapistAppService _therapistAppService;

        public EditModalModel(ITherapistAppService therapistAppService)
        {
            _therapistAppService = therapistAppService;
        }

        public async Task OnGetAsync()
        {
            var therapistDto = await _therapistAppService.GetAsync(Id);
            Therapist = ObjectMapper.Map<TherapistDto, CreateUpdateTherapistDto>(therapistDto);
            // A senha não é carregada para edição
            Therapist.Password = null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _therapistAppService.UpdateAsync(Id, Therapist);
            return NoContent();
        }
    }
}