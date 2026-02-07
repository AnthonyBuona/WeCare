using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Patients;
using WeCare.PerformedTrainings;
using WeCare.Therapists;

namespace WeCare.Consultations
{
    public class Consultation : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

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

        public string Specialty { get; set; }

        public string MainTraining { get; set; }
        public string Duration { get; set; }

        public virtual ICollection<PerformedTraining> PerformedTrainings { get; set; }

        public Guid ObjectiveId { get; set; }
    }
}