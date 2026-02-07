using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.Clinics
{
    public class Clinic : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(18)] // CNPJ: 00.000.000/0000-00
        public string? CNPJ { get; set; }

        [MaxLength(256)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Specializations { get; set; } // Lista separada por vírgulas

        public ClinicStatus Status { get; set; }

        public Clinic()
        {
            Status = ClinicStatus.Active;
        }
    }


}
