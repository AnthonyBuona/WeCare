using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Consultations;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Application.Contracts.Consultations;
using WeCare.Tratamentos;
using System.Linq;

namespace WeCare.Web.Pages.Consultations
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateConsultationDto Consultation { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime ConsultationDate { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Hora")]
        [DataType(DataType.Time)]
        public string ConsultationTime { get; set; }

        public SelectList PatientLookup { get; set; }
        public SelectList TherapistLookup { get; set; }
        public SelectList TratamentoLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly IPatientAppService _patientAppService;
        private readonly ITherapistAppService _therapistAppService;
        private readonly ITratamentoAppService _tratamentoAppService;

        public CreateModalModel(
            IConsultationAppService consultationAppService,
            IPatientAppService patientAppService,
            ITherapistAppService therapistAppService,
            ITratamentoAppService tratamentoAppService)
        {
            _consultationAppService = consultationAppService;
            _patientAppService = patientAppService;
            _therapistAppService = therapistAppService;
            _tratamentoAppService = tratamentoAppService;
        }

        public async Task OnGetAsync(DateTime? consultationDate)
        {
            Consultation = new CreateUpdateConsultationDto();

            var now = DateTime.Now;
            ConsultationDate = consultationDate.HasValue ? consultationDate.Value.Date : now.Date; 
            ConsultationTime = now.ToString("HH:mm"); 

            var patientLookupResult = await _patientAppService.GetPatientLookupAsync();
            PatientLookup = new SelectList(patientLookupResult.Items, "Id", "DisplayName");

            var therapistLookupResult = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookupResult.Items, "Id", "DisplayName");

            var tratamentoResult = await _tratamentoAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
            var tratamentos = tratamentoResult.Items.Select(x => new { Id = x.Id, DisplayName = x.Tipo + " (" + x.PatientName + ")" });
            TratamentoLookup = new SelectList(tratamentos, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (TimeSpan.TryParse(ConsultationTime, out var timeOfDay))
            {
                Consultation.DateTime = ConsultationDate.Add(timeOfDay);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Formato de hora inválido.");
                return Page();
            }

            await _consultationAppService.CreateAsync(Consultation);
            return NoContent();
        }
    }
}
