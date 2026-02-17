using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Guests
{
    public class GuestDto : FullAuditedEntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid? ResponsibleId { get; set; }
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Relationship { get; set; }
        public Guid? UserId { get; set; }
        
        // Extra properties for UI
        public string PatientName { get; set; }
    }
}
