#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace WeCare.CrossTenantAccess
{
    public class SharedAccessAuditLog : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid ConsentId { get; set; }

        public Guid UserAccessingId { get; set; }

        public string ActionPerformed { get; set; }

        public DateTime Timestamp { get; set; }

        public string AccessingIP { get; set; }

        protected SharedAccessAuditLog()
        {
        }

        public SharedAccessAuditLog(
            Guid id,
            Guid consentId,
            Guid userAccessingId,
            string actionPerformed,
            DateTime timestamp,
            string accessingIP,
            Guid? tenantId = null)
            : base(id)
        {
            ConsentId = consentId;
            UserAccessingId = userAccessingId;
            ActionPerformed = actionPerformed;
            Timestamp = timestamp;
            AccessingIP = accessingIP;
            TenantId = tenantId;
        }
    }
}
