using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Consultations;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Application.Contracts.Consultations;

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateConsultationViewModel Consultation { get; set; }

        public SelectList TherapistLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly ITherapistAppService _therapistAppService;
        private readonly IPatientAppService _patientAppService;

        public CreateModalModel(
            IConsultationAppService consultationAppService,
            ITherapistAppService therapistAppService,
            IPatientAppService patientAppService)
        {
            _consultationAppService = consultationAppService;
            _therapistAppService = therapistAppService;
            _patientAppService = patientAppService;
            Consultation = new CreateConsultationViewModel();
        }

        public async Task OnGetAsync(Guid patientId)
        {
            Consultation.PatientId = patientId;
            var patientDto = await _patientAppService.GetAsync(patientId);
            Consultation.PatientName = patientDto.Name;

            var now = DateTime.Now;
            Consultation.ConsultationDate = now.Date;
            Consultation.ConsultationTime = now.ToString("HH:mm");

            var therapistLookupResult = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookupResult.Items, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Combina a data e a hora antes de enviar para o serviço
            if (TimeSpan.TryParse(Consultation.ConsultationTime, out var timeOfDay))
            {
                var dto = new CreateUpdateConsultationDto
                {
                    PatientId = Consultation.PatientId,
                    TherapistId = Consultation.TherapistId,
                    Description = Consultation.Description,
                    DateTime = Consultation.ConsultationDate.Add(timeOfDay)
                };
                await _consultationAppService.CreateAsync(dto);
            }
            else
            {
                // Tratamento de erro se a hora for inválida
                ModelState.AddModelError(string.Empty, "Formato de hora inválido.");
                return Page();
            }

            return NoContent();
        }

        // ViewModel interna para ajudar com os campos de data e hora no formulário
        public class CreateConsultationViewModel
        {
            [HiddenInput]
            public Guid PatientId { get; set; }
            public string PatientName { get; set; }

            [Required]
            [SelectItems("TherapistLookup")]
            [Display(Name = "Terapeuta")]
            public Guid TherapistId { get; set; }

            [Required]
            [Display(Name = "Descrição")]
            [TextArea(Rows = 4)]
            public string Description { get; set; }

            [Required]
            [Display(Name = "Data")]
            [DataType(DataType.Date)]
            public DateTime ConsultationDate { get; set; }

            [Required]
            [Display(Name = "Hora")]
            [DataType(DataType.Time)]
            public string ConsultationTime { get; set; }
        }
    }
}