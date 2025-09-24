using System;
using System.ComponentModel.DataAnnotations;
using WeCare.Domain.Shared.PerformedTrainings;

namespace WeCare.Application.Contracts.PerformedTrainings
{
    public class CreateUpdatePerformedTrainingDto
    {
        [Required]
        public Guid TrainingId { get; set; } // Alterado de 'string Activity' para 'Guid TrainingId'

        [Required]
        public HelpNeededType HelpNeeded { get; set; }

        [Range(0, 100)]
        public int TotalAttempts { get; set; }

        [Range(0, 100)]
        public int SuccessfulAttempts { get; set; }
    }
}