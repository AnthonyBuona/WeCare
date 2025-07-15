using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Consultations;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Application.Contracts.Consultations;

namespace WeCare.Web.Pages.Consultations
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateConsultationDto Consultation { get; set; }

        // --- IN�CIO DA ALTERA��O ---

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

        // --- FIM DA ALTERA��O ---

        public SelectList PatientLookup { get; set; }
        public SelectList TherapistLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly IPatientAppService _patientAppService;
        private readonly ITherapistAppService _therapistAppService;

        public CreateModalModel(
            IConsultationAppService consultationAppService,
            IPatientAppService patientAppService,
            ITherapistAppService therapistAppService)
        {
            _consultationAppService = consultationAppService;
            _patientAppService = patientAppService;
            _therapistAppService = therapistAppService;
        }

        public async Task OnGetAsync()
        {
            Consultation = new CreateUpdateConsultationDto();

            // --- IN�CIO DA ALTERA��O ---
            var now = DateTime.Now;
            ConsultationDate = now.Date; // Define a data de hoje
            ConsultationTime = now.ToString("HH:mm"); // Define a hora e minuto atuais
            // --- FIM DA ALTERA��O ---

            var patientLookupResult = await _patientAppService.GetPatientLookupAsync();
            PatientLookup = new SelectList(patientLookupResult.Items, "Id", "DisplayName");

            var therapistLookupResult = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookupResult.Items, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // --- IN�CIO DA ALTERA��O: Combina data e hora antes de salvar ---
            if (TimeSpan.TryParse(ConsultationTime, out var timeOfDay))
            {
                Consultation.DateTime = ConsultationDate.Add(timeOfDay);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Formato de hora inv�lido.");
                return Page();
            }
            // --- FIM DA ALTERA��O ---

            await _consultationAppService.CreateAsync(Consultation);
            return NoContent();
        }
    }
}