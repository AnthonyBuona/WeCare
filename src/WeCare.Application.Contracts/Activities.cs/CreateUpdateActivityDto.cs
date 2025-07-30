using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Activities
{
    public class CreateUpdateActivityDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public Guid TrainingId { get; set; }
    }
}