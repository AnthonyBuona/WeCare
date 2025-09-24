using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Trainings
{
    public class CreateUpdateTrainingDto
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid ObjectiveId { get; set; } // << Adicionar este campo
    }
}