using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Patients;
using WeCare.Therapists;

namespace WeCare.Tratamentos
{
    public class Tratamento : AuditedAggregateRoot<Guid>
    {
        // Chaves Estrangeiras
        public Guid PatientId { get; set; }
        public Guid TherapistId { get; set; }

        [Required]
        [StringLength(100)]
        public string Tipo { get; set; } 

        public Patient Patient { get; set; }
        public Therapist Therapist { get; set; }
    }
}