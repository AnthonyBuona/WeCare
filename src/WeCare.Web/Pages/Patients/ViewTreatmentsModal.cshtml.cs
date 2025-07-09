using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Patients;
using WeCare.Tratamentos;

namespace WeCare.Web.Pages.Patients
{
    public class ViewTreatmentsModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid PatientId { get; set; }

        public PatientDto Patient { get; set; }

        private readonly IPatientAppService _patientAppService;

        public ViewTreatmentsModalModel(IPatientAppService patientAppService)
        {
            _patientAppService = patientAppService;
        }

        public async Task OnGetAsync()
        {
            Patient = await _patientAppService.GetAsync(PatientId);
        }
    }
}