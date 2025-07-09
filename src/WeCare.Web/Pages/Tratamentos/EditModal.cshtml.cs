using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Tratamentos;
using WeCare.Patients;
using WeCare.Therapists; 

namespace WeCare.Web.Pages.Tratamentos
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateTratamentoDto Tratamento { get; set; }

        public SelectList PatientLookup { get; set; }
        public SelectList TherapistLookup { get; set; } 

        private readonly ITratamentoAppService _TratamentoAppService;
        private readonly IPatientAppService _patientAppService;
        private readonly ITherapistAppService _therapistAppService; 

        public EditModalModel(
            ITratamentoAppService TratamentoAppService,
            IPatientAppService patientAppService,
            ITherapistAppService therapistAppService)
        {
            _TratamentoAppService = TratamentoAppService;
            _patientAppService = patientAppService;
            _therapistAppService = therapistAppService; 
        }

        public async Task OnGetAsync()
        {
            var consultaDto = await _TratamentoAppService.GetAsync(Id);
            Tratamento = ObjectMapper.Map<TratamentoDto, CreateUpdateTratamentoDto>(consultaDto);


            var patientLookup = await _patientAppService.GetPatientLookupAsync();
            PatientLookup = new SelectList(patientLookup.Items, "Id", "DisplayName", Tratamento.PatientId);

            var therapistLookup = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookup.Items, "Id", "DisplayName", Tratamento.TherapistId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _TratamentoAppService.UpdateAsync(Id, Tratamento);
            return NoContent();
        }
    }
}