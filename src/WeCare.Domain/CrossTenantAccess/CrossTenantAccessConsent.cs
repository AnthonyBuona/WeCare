#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.CrossTenantAccess
{
    public class CrossTenantAccessConsent : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid PatientId { get; set; }

        public Guid SourceTenantId { get; set; }

        public Guid TargetTenantId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string GrantedPermissions { get; set; }

        public string AuthTokenHash { get; set; }

        public bool IsRevoked { get; set; }

        protected CrossTenantAccessConsent()
        {
        }

        public CrossTenantAccessConsent(
            Guid id,
            Guid patientId,
            Guid sourceTenantId,
            Guid targetTenantId,
            DateTime expirationDate,
            string grantedPermissions,
            string authTokenHash,
            bool isRevoked = false,
            Guid? tenantId = null)
            : base(id)
        {
            PatientId = patientId;
            SourceTenantId = sourceTenantId;
            TargetTenantId = targetTenantId;
            ExpirationDate = expirationDate;
            GrantedPermissions = grantedPermissions;
            AuthTokenHash = authTokenHash;
            IsRevoked = isRevoked;
            TenantId = tenantId ?? sourceTenantId; // Set TenantId to sourceTenantId if not provided to respect multitenancy context
        }
    }
}
