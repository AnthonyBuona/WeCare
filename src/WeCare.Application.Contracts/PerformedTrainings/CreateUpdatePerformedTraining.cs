using System.ComponentModel.DataAnnotations;

namespace WeCare.Application.Contracts.PerformedTrainings
{
    public class CreateUpdatePerformedTrainingDto
    {
        [Required]
        public string Activity { get; set; }
        public string HelpNeeded { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
    }
}