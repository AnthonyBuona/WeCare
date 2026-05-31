using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Gamification
{
    public class CaregiverQuestDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid PatientId { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public string VideoTutorialUrl { get; set; }
        public int XpReward { get; set; }
    }
}
