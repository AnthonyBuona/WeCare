using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Trainings; // Adicione este using

namespace WeCare.Activities
{
    public class Activity : AuditedAggregateRoot<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
        public Guid ObjectiveId { get; set; }

        public Guid TrainingId { get; set; } 
        public virtual Training Training { get; set; } 
    }
}