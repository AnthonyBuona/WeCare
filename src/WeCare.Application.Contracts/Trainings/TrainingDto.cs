using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WeCare.Activities;

namespace WeCare.Trainings
{
    public class TrainingDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }
}