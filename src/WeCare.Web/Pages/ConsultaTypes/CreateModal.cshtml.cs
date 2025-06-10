using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Consultas;
using WeCare.Patients; // Adicione este using

namespace WeCare.Web.Pages.Tratamentos
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateTratamentoDto Tratamento { get; set; }

        // Adicione esta propriedade
        public SelectList PatientLookup { get; set; }
        // public SelectList TherapistLookup { get; set; } // Você adicionará isso mais tarde

        private readonly ITratamentoAppService _TratamentoAppService;
        private readonly IPatientAppService _patientAppService; // Injete o serviço de lookup

        public CreateModalModel(
            ITratamentoAppService TratamentoAppService,
            IPatientAppService patientAppService) // Adicione aqui
        {
            _TratamentoAppService = TratamentoAppService;
            _patientAppService = patientAppService; // Adicione aqui
        }

        public async Task OnGetAsync()
        {
            Tratamento = new CreateUpdateTratamentoDto();

            // Busque os pacientes
            var patientLookup = await _patientAppService.GetPatientLookupAsync();
            PatientLookup = new SelectList(patientLookup.Items, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _TratamentoAppService.CreateAsync(Tratamento);
            return NoContent();
        }
    }
}