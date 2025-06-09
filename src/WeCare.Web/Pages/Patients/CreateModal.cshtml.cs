using System.Threading.Tasks;
using WeCare.Patients;
using Microsoft.AspNetCore.Mvc;

namespace WeCare.Web.Pages.Patients
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdatePatientDto Patient { get; set; }

        private readonly IPatientAppService _PatientAppService;

        public CreateModalModel(IPatientAppService PatientAppService)
        {
            _PatientAppService = PatientAppService;
        }

        public void OnGet()
        {
            Patient = new CreateUpdatePatientDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _PatientAppService.CreateAsync(Patient);
            return NoContent();
        }
    }
}