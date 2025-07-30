using System;
using System.Collections.Generic; // Adicione este using
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Activities; // Adicione este using

namespace WeCare.Trainings
{
    public class Training : AuditedAggregateRoot<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
        public virtual ICollection<Activities.Activity> Activities { get; set; }
        public Training()
        {
            Activities = new HashSet<Activities.Activity>();
        }
    }
}