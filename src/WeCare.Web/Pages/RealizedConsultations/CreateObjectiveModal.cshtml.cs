using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Application.Contracts.Consultations;
using WeCare.Therapists;
using WeCare.Objectives;

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class CreateObjectiveModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateObjectiveViewModel Objective { get; set; }

        public SelectList TherapistLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly IObjectiveAppService _objectiveAppService;
        private readonly ITherapistAppService _therapistAppService;

        public CreateObjectiveModalModel(
            IConsultationAppService consultationAppService,
            IObjectiveAppService objectiveAppService,
            ITherapistAppService therapistAppservice)
        {
            _consultationAppService = consultationAppService;
            _objectiveAppService = objectiveAppService;
            _therapistAppService = therapistAppservice;
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

        // NOVO MÉTODO: Busca a especialidade do terapeuta para o JavaScript
        public async Task<JsonResult> OnGetSpecialtyAsync(Guid therapistId)
        {
            var therapist = await _therapistAppService.GetAsync(therapistId);
            return new JsonResult(new { specialty = therapist.Specialization });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!TimeSpan.TryParse(Objective.FirstConsultationTime, out _))
            {
                ModelState.AddModelError(nameof(Objective.FirstConsultationTime), "Formato de hora inválido. Use HH:mm.");
            }

            if (!ModelState.IsValid)
            {
                // A validação do modelo é tratada automaticamente pelo ABP Framework.
            }

            var timeOfDay = TimeSpan.Parse(Objective.FirstConsultationTime);

            // ALTERAÇÃO: Busca o terapeuta para obter a especialidade diretamente do banco,
            // garantindo a integridade dos dados.
            var therapist = await _therapistAppService.GetAsync(Objective.TherapistId);

            var createDto = new CreateUpdateObjectiveDto
            {
                PatientId = Objective.PatientId,
                Name = Objective.ObjectiveName,
                TherapistId = Objective.TherapistId,
                StartDate = Objective.FirstConsultationDate.Add(timeOfDay),
                Status = "Ativo",
            };

            await _objectiveAppService.CreateAsync(createDto);

            return NoContent();
        }

        public class CreateObjectiveViewModel
        {
            [HiddenInput]
            public Guid PatientId { get; set; }

            [Required]
            [Display(Name = "Nome do Objetivo")]
            public string ObjectiveName { get; set; }

            // ALTERAÇÃO: O campo agora é apenas para exibição, a validação foi removida.
            [Display(Name = "Especialidade")]
            public string Specialty { get; set; }

            [Required]
            [SelectItems("TherapistLookup")]
            [Display(Name = "Terapeuta Responsável")]
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