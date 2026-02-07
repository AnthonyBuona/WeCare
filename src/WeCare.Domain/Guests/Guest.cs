using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Patients;
using WeCare.Responsibles;

namespace WeCare.Guests
{
    public class Guest : FullAuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        [Required]
        public Guid ResponsibleId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        public Guid? UserId { get; set; }

        // Navigation properties
        public virtual Responsible Responsible { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
