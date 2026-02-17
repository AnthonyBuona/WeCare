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

        // White-Label
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }

        // Address & Contact
        public string? AddressNumber { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? WelcomeMessage { get; set; }

        // Scheduling
        public int AppointmentDurationMinutes { get; set; }
        public System.Collections.Generic.List<ClinicOperatingHourDto> OperatingHours { get; set; }
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
