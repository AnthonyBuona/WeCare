using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.CrossTenantAccess
{
    public class SharedAccessAuditLogDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid ConsentId { get; set; }
        public Guid UserAccessingId { get; set; }
        public string ActionPerformed { get; set; }
        public DateTime Timestamp { get; set; }
        public string AccessingIP { get; set; }
    }
}
