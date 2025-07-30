using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Application.Contracts.Consultations;
using WeCare.Patients;
using WeCare.Therapists;

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public RegisterConsultationViewModel Consultation { get; set; }

        // Propriedades para popular os dropdowns
        public SelectList MainTrainingLookup { get; set; }
        public SelectList ActivityLookup { get; set; }
        public SelectList HelpNeededLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;
        private readonly IPatientAppService _patientAppService;

        public CreateModalModel(
            IConsultationAppService consultationAppService,
            IPatientAppService patientAppService)
        {
            _consultationAppService = consultationAppService;
            _patientAppService = patientAppService;
            Consultation = new RegisterConsultationViewModel();
        }

        public async Task OnGetAsync(Guid patientId)
        {
            var patientDto = await _patientAppService.GetAsync(patientId);

            Consultation = new RegisterConsultationViewModel
            {
                PatientId = patientId,
                PatientName = patientDto.Name,
                ConsultationDate = DateTime.Now.Date
            };

            // Simulação de dados para os dropdowns
            // TODO: Substituir por uma busca real no banco de dados, se necessário
            MainTrainingLookup = new SelectList(new List<string> { "Treino A", "Treino B", "Treino C" });
            ActivityLookup = new SelectList(new List<string> { "Atividade 1", "Atividade 2", "Atividade 3" });
            HelpNeededLookup = new SelectList(new List<string> { "Nenhuma", "Pouca", "Muita" });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // TODO: Mapear o RegisterConsultationViewModel para o seu CreateUpdateConsultationDto
            // que agora deve aceitar uma lista de treinos realizados.
            // Exemplo:
            // var createDto = ObjectMapper.Map<RegisterConsultationViewModel, CreateUpdateConsultationDto>(Consultation);
            // await _consultationAppService.CreateAsync(createDto);

            // A lógica de mapeamento e salvamento dependerá das alterações no seu AppService.
            await Task.CompletedTask; // Remover após implementar a lógica de salvamento

            return NoContent();
        }
    }

    // ViewModel para o formulário principal
    public class RegisterConsultationViewModel
    {
        [HiddenInput]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }

        [Display(Name = "Treino realizado")]
        [SelectItems(nameof(CreateModalModel.MainTrainingLookup))]
        public string MainTraining { get; set; }

        [Display(Name = "Duração")]
        public string Duration { get; set; }

        [Required]
        [Display(Name = "Data da última consulta")]
        [DataType(DataType.Date)]
        public DateTime ConsultationDate { get; set; }

        [Display(Name = "Comentários")]
        [TextArea(Rows = 3)]
        public string Comments { get; set; }

        public List<PerformedTrainingViewModel> PerformedTrainings { get; set; } = new();
    }

    // ViewModel para cada item na lista de treinos
    public class PerformedTrainingViewModel
    {
        [Display(Name = "Atividade que foi realizada")]
        [SelectItems(nameof(CreateModalModel.ActivityLookup))]
        public string Activity { get; set; }

        [Display(Name = "Ajuda necessária")]
        [SelectItems(nameof(CreateModalModel.HelpNeededLookup))]
        public string HelpNeeded { get; set; }

        [Display(Name = "Número de tentativas")]
        public int TotalAttempts { get; set; }

        [Display(Name = "Tentativas bem sucedidas")]
        public int SuccessfulAttempts { get; set; }
    }
}