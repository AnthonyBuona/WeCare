using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Objectives;

namespace WeCare.Trainings
{
    public class Training : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        [ForeignKey(nameof(Objective))]
        public Guid ObjectiveId { get; set; }
        public virtual Objective Objective { get; set; } = null!;
    }
}