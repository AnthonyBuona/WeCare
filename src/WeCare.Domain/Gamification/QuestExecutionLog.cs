#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace WeCare.Gamification
{
    public class QuestExecutionLog : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid QuestId { get; set; }

        public DateTime ExecutionDate { get; set; }

        public int EnjoymentScore { get; set; }

        public string CaregiverNotes { get; set; }

        protected QuestExecutionLog()
        {
        }

        public QuestExecutionLog(
            Guid id,
            Guid questId,
            DateTime executionDate,
            int enjoymentScore,
            string caregiverNotes,
            Guid? tenantId = null)
            : base(id)
        {
            QuestId = questId;
            ExecutionDate = executionDate;
            EnjoymentScore = enjoymentScore;
            CaregiverNotes = caregiverNotes;
            TenantId = tenantId;
        }
    }
}
