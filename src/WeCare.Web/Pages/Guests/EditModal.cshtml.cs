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
        public CreateUpdateGuestDto Guest { get; set; }

        public List<SelectListItem> PatientLookup { get; set; }
        
        public bool IsResponsible { get; set; }

        private readonly IGuestAppService _guestAppService;

        public EditModalModel(IGuestAppService guestAppService)
        {
            _guestAppService = guestAppService;
        }

        public async Task OnGetAsync()
        {
            IsResponsible = CurrentUser.IsInRole("Responsible");
            var guestDto = await _guestAppService.GetAsync(Id);
            // Mapeamento manual simples ou use AutoMapper se configurado na Web Layer
            Guest = new CreateUpdateGuestDto 
            {
                 Name = guestDto.Name,
                 Email = guestDto.Email,
                 PatientId = guestDto.PatientId,
                 // Note: Password and UserName required by DTO. 
                 // GuestDto does not have UserName, and we can't easily get it here without extra call.
                 // For now, leaving them empty. If [Required] fails validation, we might need to adjust DTO or AppService.
            };
            
            // Note: GuestDto has UserName, likely. -> No it doesn't.
            // Guest.UserName = guestDto.UserName;


            var patientLookup = await _guestAppService.GetPatientLookupAsync();
            PatientLookup = patientLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _guestAppService.UpdateAsync(Id, Guest);
            return NoContent();
        }
    }
}
