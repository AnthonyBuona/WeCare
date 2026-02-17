using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WeCare.Guests;
using Volo.Abp.Domain.Repositories;

namespace WeCare.Web.Pages.Guests
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateGuestDto Guest { get; set; }

        private readonly IGuestAppService _guestAppService;
        private readonly Volo.Abp.Domain.Repositories.IRepository<WeCare.Patients.Patient, System.Guid> _patientRepository;
        private readonly Volo.Abp.Domain.Repositories.IRepository<WeCare.Responsibles.Responsible, System.Guid> _responsibleRepository;

        public List<SelectListItem> PatientLookupList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ResponsibleLookupList { get; set; } = new List<SelectListItem>();
        
        public bool IsResponsible { get; set; }

        public CreateModalModel(
            IGuestAppService guestAppService,
            Volo.Abp.Domain.Repositories.IRepository<WeCare.Patients.Patient, System.Guid> patientRepository,
            Volo.Abp.Domain.Repositories.IRepository<WeCare.Responsibles.Responsible, System.Guid> responsibleRepository)
        {
            _guestAppService = guestAppService;
            _patientRepository = patientRepository;
            _responsibleRepository = responsibleRepository;
            Guest = new CreateUpdateGuestDto();
        }

        public async Task OnGetAsync()
        {
            Guest = new CreateUpdateGuestDto();
            IsResponsible = CurrentUser.IsInRole("Responsible");

            if (IsResponsible)
            {
                var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
                if (responsible != null)
                {
                    Guest.ResponsibleId = responsible.Id;
                    
                    var patient = await _patientRepository.FirstOrDefaultAsync(p => p.PrincipalResponsibleId == responsible.Id);
                    if (patient != null)
                    {
                        Guest.PatientId = patient.Id;
                    }
                }
            }
            
            var patientLookup = await _guestAppService.GetPatientLookupAsync();
            PatientLookupList = patientLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();

            var responsibleLookup = await _guestAppService.GetResponsibleLookupAsync();
            ResponsibleLookupList = responsibleLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _guestAppService.CreateAsync(Guest);
            return NoContent();
        }
    }
}
