using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Guests;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal;

namespace WeCare.Web.Pages.Guests
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateGuestDto Guest { get; set; }

        public List<SelectListItem> PatientLookup { get; set; }

        private readonly IGuestAppService _guestAppService;

        public EditModalModel(IGuestAppService guestAppService)
        {
            _guestAppService = guestAppService;
        }

        public async Task OnGetAsync()
        {
            var guestDto = await _guestAppService.GetAsync(Id);
            // Mapeamento manual simples ou use AutoMapper se configurado na Web Layer
            Guest = new CreateGuestDto 
            {
                 Name = guestDto.Name,
                 Email = guestDto.Email,
                 PatientId = guestDto.PatientId
            };

            var patientLookup = await _guestAppService.GetMyPatientsLookupAsync();
            PatientLookup = patientLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // O serviço de aplicação precisa suportar Update. Verifique se ICrudAppService suporta ou se precisa de UpdateGuestDto.
            // IGuestAppService herda de ICrudAppService<GuestDto, Guid, PagedAndSortedResultRequestDto, CreateGuestDto>
            // Então o tipo de entrada para Create e Update é o mesmo: CreateGuestDto.
            
            await _guestAppService.UpdateAsync(Id, Guest);
            return NoContent();
        }
    }
}
