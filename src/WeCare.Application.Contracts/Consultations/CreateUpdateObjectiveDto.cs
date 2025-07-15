using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Application.Contracts.Consultations
{
    public class CreateUpdateObjectiveDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [StringLength(500)]
        public string ObjectiveName { get; set; }

        [Required]
        public Guid TherapistId { get; set; }

        [Required]
        public DateTime FirstConsultationDateTime { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }
    }
}