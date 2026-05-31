using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.CrossTenantAccess
{
    public class CreateCrossTenantAccessConsentDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid TargetTenantId { get; set; }

        [Required]
        public int ExpirationDays { get; set; }

        [Required]
        public string GrantedPermissions { get; set; }
    }
}
