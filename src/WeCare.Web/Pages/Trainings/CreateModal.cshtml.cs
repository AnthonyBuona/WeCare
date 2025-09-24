using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Trainings;

namespace WeCare.Web.Pages.Trainings
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateTrainingViewModel Training { get; set; }

        private readonly ITrainingAppService _trainingAppService;

        public CreateModalModel(ITrainingAppService trainingAppService)
        {
            _trainingAppService = trainingAppService;
            Training = new CreateTrainingViewModel();
        }

        public void OnGet(Guid objectiveId)
        {
            Training.ObjectiveId = objectiveId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<CreateTrainingViewModel, CreateUpdateTrainingDto>(Training);
            var createdTraining = await _trainingAppService.CreateAsync(dto);
            return new OkObjectResult(createdTraining); // Retorna o treino criado para o JS
        }
    }

    public class CreateTrainingViewModel
    {
        [Required]
        [Display(Name = "Nome do Treino")]
        public string Name { get; set; }

        [HiddenInput]
        public Guid ObjectiveId { get; set; }
    }
}