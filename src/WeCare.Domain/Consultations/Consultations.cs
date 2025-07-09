using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Patients;
using WeCare.Therapists;

namespace WeCare.Consultations
{
    public class Consultation : AuditedAggregateRoot<Guid>
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid TherapistId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public Patient Patient { get; set; }
        public Therapist Therapist { get; set; }
    }
}