using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.CrossTenantAccess
{
    public class CrossTenantAccessConsentDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid PatientId { get; set; }
        public Guid SourceTenantId { get; set; }
        public Guid TargetTenantId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string GrantedPermissions { get; set; }
        public string AuthTokenHash { get; set; }
        public bool IsRevoked { get; set; }
        public string RawToken { get; set; } // Only populated when creating
    }
}
