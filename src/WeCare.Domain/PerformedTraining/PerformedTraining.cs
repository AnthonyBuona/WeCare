using System;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Consultations;

namespace WeCare.PerformedTrainings
{
    public class PerformedTraining : FullAuditedEntity<Guid>
    {
        public string Activity { get; set; }
        public string HelpNeeded { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }

        // Chave Estrangeira para a Consulta
        public Guid ConsultationId { get; set; }
        public virtual Consultation Consultation { get; set; }
    }
}