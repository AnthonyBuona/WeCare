using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Consultations;
using WeCare.Trainings;
using WeCare.Domain.Shared.PerformedTrainings;

namespace WeCare.PerformedTrainings
{
    public class PerformedTraining : FullAuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid TrainingId { get; set; }
        public virtual Training Training { get; set; }
        public HelpNeededType HelpNeeded { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }

        // Chave Estrangeira para a Consulta
        public Guid ConsultationId { get; set; }
        public virtual Consultation Consultation { get; set; }
    }
}