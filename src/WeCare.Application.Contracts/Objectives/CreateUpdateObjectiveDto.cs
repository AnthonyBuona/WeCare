// Local: WeCare.Application.Contracts/Objectives/CreateUpdateObjectiveDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Objectives
{
    public class CreateUpdateObjectiveDto
    {
        [Required]
        public Guid PatientId { get; set; }
        [Required]
        public Guid TherapistId { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}