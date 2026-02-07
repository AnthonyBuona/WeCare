using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace WeCare.Clinics
{
    public class ClinicDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string? CNPJ { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Specializations { get; set; }
        public ClinicStatus Status { get; set; }
    }

    public class CreateUpdateClinicDto
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(18)]
        public string? CNPJ { get; set; }

        [MaxLength(256)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Specializations { get; set; }
    }

    public class ChangeClinicStatusDto
    {
        [Required]
        public ClinicStatus Status { get; set; }
    }
}
