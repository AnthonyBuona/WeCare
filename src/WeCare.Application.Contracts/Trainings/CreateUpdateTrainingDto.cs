using System.ComponentModel.DataAnnotations;

namespace WeCare.Trainings
{
    public class CreateUpdateTrainingDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}