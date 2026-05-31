using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Gamification
{
    public class QuestExecutionLogDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid QuestId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int EnjoymentScore { get; set; }
        public string CaregiverNotes { get; set; }
    }
}
