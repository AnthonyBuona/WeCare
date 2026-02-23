using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WeCare.Therapists;

namespace WeCare.Web.Pages.Therapists
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateTherapistDto Therapist { get; set; }

        public List<SelectListItem> SpecializationList { get; set; } = new();

        private readonly ITherapistAppService _therapistAppService;
        private readonly WeCare.Clinics.IClinicAppService _clinicAppService;

        public CreateModalModel(
            ITherapistAppService therapistAppService,
            WeCare.Clinics.IClinicAppService clinicAppService)
        {
            _therapistAppService = therapistAppService;
            _clinicAppService = clinicAppService;
            Therapist = new CreateUpdateTherapistDto();
        }

        public async Task OnGetAsync()
        {
            var settings = await _clinicAppService.GetCurrentClinicSettingsAsync();
            if (settings?.Specializations != null)
            {
                SpecializationList = settings.Specializations
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => new SelectListItem(s, s))
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _therapistAppService.CreateAsync(Therapist);
            return NoContent();
        }
    }
}