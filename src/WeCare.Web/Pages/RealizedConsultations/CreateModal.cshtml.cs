using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Application.Contracts.Consultations;
using WeCare.Application.Contracts.PerformedTrainings;
using WeCare.Domain.Shared.PerformedTrainings;
using WeCare.Consultations;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Objectives;
using WeCare.Trainings;
using WeCare.Tratamentos;

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public RegisterConsultationViewModel Consultation { get; set; } = null!;

        public SelectList ObjectiveLookup { get; set; } = null!;
        public SelectList TrainingLookup { get; set; } = null!;
        public SelectList HelpNeededLookup { get; set; } = null!;
        public SelectList TratamentoLookup { get; set; } = null!;

        private readonly IConsultationAppService _consultationAppService;
        private readonly IPatientAppService _patientAppService;
        private readonly IObjectiveAppService _objectiveAppService;
        private readonly ITrainingAppService _trainingAppService;
        private readonly ITherapistAppService _therapistAppService;
        private readonly ITratamentoAppService _tratamentoAppService;

        public CreateModalModel(
            IConsultationAppService consultationAppService,
            IPatientAppService patientAppService,
            IObjectiveAppService objectiveAppService,
            ITrainingAppService trainingAppService,
            ITherapistAppService therapistAppService,
            ITratamentoAppService tratamentoAppService)
        {
            _consultationAppService = consultationAppService;
            _patientAppService = patientAppService;
            _objectiveAppService = objectiveAppService;
            _trainingAppService = trainingAppService;
            _therapistAppService = therapistAppService;
            _tratamentoAppService = tratamentoAppService;
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

            // Carregar objetivos do paciente
            var objectiveLookupResult = await _objectiveAppService.GetObjectiveLookupAsync(patientId);
            ObjectiveLookup = new SelectList(objectiveLookupResult.Items, "Id", "DisplayName");

            // Carregar treinos (todos disponíveis)
            var trainings = await _trainingAppService.GetListAsync(
                new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
            TrainingLookup = new SelectList(trainings.Items, "Id", "Name");

            // Carregar tipos de ajuda do enum HelpNeededType
            var helpNeededItems = Enum.GetValues(typeof(HelpNeededType))
                .Cast<HelpNeededType>()
                .Select(e => new { Value = e.ToString(), Text = GetHelpNeededDisplayName(e) })
                .ToList();
            HelpNeededLookup = new SelectList(helpNeededItems, "Value", "Text");

            var tratamentoResult = await _tratamentoAppService.GetListByPatient(patientId, new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 100 });
            var tratamentos = tratamentoResult.Items.Select(x => new { Id = x.Id, DisplayName = x.Tipo + " (" + x.TherapistName + ")" });
            TratamentoLookup = new SelectList(tratamentos, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Buscar o objetivo selecionado para obter o TherapistId correto
            var objective = await _objectiveAppService.GetAsync(Consultation.ObjectiveId);
            var therapistId = objective.TherapistId;

            var createDto = new CreateUpdateConsultationDto
            {
                PatientId = Consultation.PatientId,
                TherapistId = therapistId,
                TratamentoId = Consultation.TratamentoId,
                DateTime = Consultation.ConsultationDate,
                Description = Consultation.Comments ?? "Consulta realizada",
                MainTraining = Consultation.MainTraining ?? string.Empty,
                Duration = Consultation.Duration.ToString() + " min",
                ObjectiveId = Consultation.ObjectiveId,
                Status = ConsultationStatus.Realizada,
                PerformedTrainings = Consultation.PerformedTrainings?.Select(pt => new CreateUpdatePerformedTrainingDto
                {
                    TrainingId = pt.TrainingId,
                    HelpNeeded = Enum.TryParse<HelpNeededType>(pt.HelpNeeded, out var helpType) ? helpType : HelpNeededType.I,
                    TotalAttempts = pt.TotalAttempts,
                    SuccessfulAttempts = pt.SuccessfulAttempts
                }).ToList() ?? new List<CreateUpdatePerformedTrainingDto>()
            };

            await _consultationAppService.CreateAsync(createDto);

            return NoContent();
        }

        private string GetHelpNeededDisplayName(HelpNeededType type)
        {
            return type switch
            {
                HelpNeededType.I => "Independente",
                HelpNeededType.SV => "Suporte Verbal",
                HelpNeededType.SG => "Suporte Gestual",
                HelpNeededType.SP => "Suporte Posicional",
                HelpNeededType.ST => "Suporte Total",
                _ => type.ToString()
            };
        }
    }

    // ViewModel para o formulario principal
    public class RegisterConsultationViewModel
    {
        [HiddenInput]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = null!;

        [Required]
        [SelectItems(nameof(CreateModalModel.ObjectiveLookup))]
        [Display(Name = "Objetivo")]
        public Guid ObjectiveId { get; set; }

        [Required]
        [SelectItems(nameof(CreateModalModel.TratamentoLookup))]
        [Display(Name = "Tratamento")]
        public Guid TratamentoId { get; set; }

        [Display(Name = "Treino realizado")]
        public string? MainTraining { get; set; }

        [Required]
        [Display(Name = "Duração (minutos)")]
        public int Duration { get; set; }

        [Required]
        [Display(Name = "Data da consulta")]
        [DataType(DataType.Date)]
        public DateTime ConsultationDate { get; set; }

        [Display(Name = "Comentários")]
        [TextArea(Rows = 3)]
        public string? Comments { get; set; }

        public List<PerformedTrainingViewModel> PerformedTrainings { get; set; } = new();
    }

    // ViewModel para cada item na lista de treinos
    public class PerformedTrainingViewModel
    {
        [Display(Name = "Treino")]
        public Guid TrainingId { get; set; }

        [Display(Name = "Ajuda necessária")]
        public string HelpNeeded { get; set; } = null!;

        [Display(Name = "Número de tentativas")]
        public int TotalAttempts { get; set; }

        [Display(Name = "Tentativas bem sucedidas")]
        public int SuccessfulAttempts { get; set; }
    }
}
