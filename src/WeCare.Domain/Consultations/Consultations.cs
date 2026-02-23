#pragma warning disable CS8618
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Patients;
using WeCare.PerformedTrainings;
using WeCare.Therapists;
using WeCare.Tratamentos;

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
        public Guid TratamentoId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public Patient Patient { get; set; }
        public Therapist Therapist { get; set; }
        public Tratamento Tratamento { get; set; }

        public string Specialty { get; set; }

        public string MainTraining { get; set; }
        public string Duration { get; set; }

        /// <summary>
        /// Status da consulta: Agendada (padrão) ou Realizada.
        /// </summary>
        public ConsultationStatus Status { get; set; } = ConsultationStatus.Agendada;

        public virtual ICollection<PerformedTraining> PerformedTrainings { get; set; }

        /// <summary>
        /// Objetivo principal (opcional). Consultas agendadas não precisam de objetivo.
        /// Os objetivos são vinculados indiretamente via PerformedTrainings → Training → Objective.
        /// </summary>
        public Guid? ObjectiveId { get; set; }
    }
}
