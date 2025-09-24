using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Objectives;

namespace WeCare.Trainings
{
    public class Training : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey(nameof(Objective))]
        public Guid ObjectiveId { get; set; }
        public virtual Objective Objective { get; set; }
    }
}