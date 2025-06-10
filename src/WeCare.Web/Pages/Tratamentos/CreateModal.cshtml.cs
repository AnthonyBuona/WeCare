using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Tratamentos;
using WeCare.Patients;
using WeCare.Therapists; // Adicione este using

namespace WeCare.Web.Pages.Tratamentos
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateTratamentoDto Tratamento { get; set; }

        // Propriedades para popular os dropdowns
        public SelectList PatientLookup { get; set; }
        public SelectList TherapistLookup { get; set; }

        private readonly ITratamentoAppService _tratamentoAppService;
        private readonly IPatientAppService _patientAppService;
        private readonly ITherapistAppService _therapistAppService; // Injete o serviço de terapeutas

        public CreateModalModel(
            ITratamentoAppService tratamentoAppService,
            IPatientAppService patientAppService,
            ITherapistAppService therapistAppService) // Adicione aqui
        {
            _tratamentoAppService = tratamentoAppService;
            _patientAppService = patientAppService;
            _therapistAppService = therapistAppService; // Adicione aqui
        }

        public async Task OnGetAsync()
        {
            Tratamento = new CreateUpdateTratamentoDto();

            // Busca a lista de pacientes
            var patientLookupResult = await _patientAppService.GetPatientLookupAsync();
            PatientLookup = new SelectList(patientLookupResult.Items, "Id", "DisplayName");

            // Busca a lista de terapeutas
            var therapistLookupResult = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookupResult.Items, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _tratamentoAppService.CreateAsync(Tratamento);
            return NoContent();
        }
    }
}