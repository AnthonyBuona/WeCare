using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Application.Contracts.Consultations;
using WeCare.Therapists; // Referência para ITherapistAppService

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class CreateObjectiveModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateObjectiveViewModel Objective { get; set; }

        public SelectList TherapistLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly ITherapistAppService _therapistAppService;

        public CreateObjectiveModalModel(
            IConsultationAppService consultationAppService,
            ITherapistAppService therapistAppService)
        {
            _consultationAppService = consultationAppService;
            _therapistAppService = therapistAppService;
        }

        public async Task OnGetAsync(Guid patientId)
        {
            Objective = new CreateObjectiveViewModel
            {
                PatientId = patientId,
                FirstConsultationDate = DateTime.Now.Date,
                FirstConsultationTime = DateTime.Now.ToString("HH:mm")
            };

            var therapistLookupResult = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookupResult.Items, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (TimeSpan.TryParse(Objective.FirstConsultationTime, out var timeOfDay))
            {
                // Mapeia do ViewModel para o DTO que será enviado para a camada de aplicação
                var createDto = new CreateUpdateObjectiveDto
                {
                    PatientId = Objective.PatientId,
                    ObjectiveName = Objective.ObjectiveName,
                    TherapistId = Objective.TherapistId,
                    Specialty = Objective.Specialty,
                    FirstConsultationDateTime = Objective.FirstConsultationDate.Add(timeOfDay)
                };

                await _consultationAppService.CreateObjectiveAsync(createDto);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Formato de hora inválido.");
                return Page();
            }

            return NoContent();
        }

        // ViewModel para o formulário do modal
        public class CreateObjectiveViewModel
        {
            [HiddenInput]
            public Guid PatientId { get; set; }

            [Required]
            [Display(Name = "Nome do Objetivo")]
            public string ObjectiveName { get; set; }

            [Required]
            [Display(Name = "Especialidade")]
            public string Specialty { get; set; }

            [Required]
            [SelectItems("TherapistLookup")]
            [Display(Name = "Primeiro Terapeuta")]
            public Guid TherapistId { get; set; }

            [Required]
            [Display(Name = "Data da Primeira Consulta")]
            [DataType(DataType.Date)]
            public DateTime FirstConsultationDate { get; set; }

            [Required]
            [Display(Name = "Hora da Primeira Consulta")]
            [DataType(DataType.Time)]
            public string FirstConsultationTime { get; set; }
        }
    }
}