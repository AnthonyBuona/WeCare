using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Activities
{
    public class ActivityDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid TrainingId { get; set; }
    }
}