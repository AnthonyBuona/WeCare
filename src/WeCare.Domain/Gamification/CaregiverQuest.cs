#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.Gamification
{
    public class CaregiverQuest : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid PatientId { get; set; }

        public string Title { get; set; }

        public string Instructions { get; set; }

        public string VideoTutorialUrl { get; set; }

        public int XpReward { get; set; }

        protected CaregiverQuest()
        {
        }

        public CaregiverQuest(
            Guid id,
            Guid patientId,
            string title,
            string instructions,
            string videoTutorialUrl,
            int xpReward,
            Guid? tenantId = null)
            : base(id)
        {
            PatientId = patientId;
            Title = title;
            Instructions = instructions;
            VideoTutorialUrl = videoTutorialUrl;
            XpReward = xpReward;
            TenantId = tenantId;
        }
    }
}
