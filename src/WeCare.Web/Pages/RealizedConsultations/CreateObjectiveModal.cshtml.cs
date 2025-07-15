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

        [HiddenInput]
        public Guid PatientId { get; set; }

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
            if (!TimeSpan.TryParse(Objective.FirstConsultationTime, out _))
            {
                ModelState.AddModelError(nameof(Objective.FirstConsultationTime), "Formato de hora inválido. Use HH:mm.");
            }

            // Valide outras regras de negócio aqui e adicione ao ModelState se necessário

            if (!ModelState.IsValid)
            {
                // O framework do ABP/ASP.NET Core irá capturar o ModelState inválido
                // e retornar um BadRequest (400) com os erros, que o modal irá exibir.
                // Não precisamos de um 'return' explícito aqui, mas é bom para clareza.
                // A mágica do ABP já faz o trabalho.
            }

            // Se chegou até aqui, o modelo é válido
            var timeOfDay = TimeSpan.Parse(Objective.FirstConsultationTime); // Seguro fazer o Parse agora
            var createDto = new CreateUpdateObjectiveDto
            {
                PatientId = Objective.PatientId,
                ObjectiveName = Objective.ObjectiveName,
                TherapistId = Objective.TherapistId,
                Specialty = Objective.Specialty,
                FirstConsultationDateTime = Objective.FirstConsultationDate.Add(timeOfDay)
            };

            await _consultationAppService.CreateObjectiveAsync(createDto);

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