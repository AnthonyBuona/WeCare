using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Trainings;

namespace WeCare.Web.Pages.Trainings
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public EditTrainingViewModel Training { get; set; }

        private readonly ITrainingAppService _trainingAppService;

        public EditModalModel(ITrainingAppService trainingAppService)
        {
            _trainingAppService = trainingAppService;
        }

        public async Task OnGetAsync()
        {
            var trainingDto = await _trainingAppService.GetAsync(Id);
            Training = new EditTrainingViewModel
            {
                Name = trainingDto.Name,
                Description = trainingDto.Description
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dto = new CreateUpdateTrainingDto
            {
                Name = Training.Name,
                Description = Training.Description,
                ObjectiveId = Training.ObjectiveId
            };
            await _trainingAppService.UpdateAsync(Id, dto);
            return NoContent();
        }
    }

    public class EditTrainingViewModel
    {
        [Required]
        [Display(Name = "Nome do Treino")]
        [StringLength(128)]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [HiddenInput]
        public Guid ObjectiveId { get; set; }
    }
}
