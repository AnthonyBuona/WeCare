using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Gamification
{
    public class UserGamifiedProfileDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid ParentUserId { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalXp { get; set; }
        public int ActiveStreak { get; set; }
        public string UnlockedBadgesJson { get; set; }
    }
}
