#pragma warning disable CS8618
using System;
using System.Collections.Generic;
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
        public string? Specializations { get; set; } // Lista separada por vÃ­rgulas

        public ClinicStatus Status { get; set; }

        // White-Label
        [MaxLength(500)]
        public string? LogoUrl { get; set; }
        
        [MaxLength(10)]
        public string? PrimaryColor { get; set; } // Hex Code
        
        [MaxLength(10)]
        public string? SecondaryColor { get; set; } // Hex Code

        // Address & Contact
        [MaxLength(20)]
        public string? AddressNumber { get; set; }
        
        [MaxLength(100)]
        public string? Neighborhood { get; set; }
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(50)]
        public string? State { get; set; }
        
        [MaxLength(20)]
        public string? ZipCode { get; set; }

        [MaxLength(500)]
        public string? WebsiteUrl { get; set; }
        
        [MaxLength(500)]
        public string? InstagramUrl { get; set; }
        
        [MaxLength(500)]
        public string? FacebookUrl { get; set; }
        
        [MaxLength(500)]
        public string? LinkedInUrl { get; set; }
        
        [MaxLength(1000)]
        public string? WelcomeMessage { get; set; }

        // Scheduling
        public int AppointmentDurationMinutes { get; set; } = 30; // Default duration

        public ICollection<ClinicOperatingHour> OperatingHours { get; set; }

        public Clinic()
        {
            Status = ClinicStatus.Active;
            OperatingHours = new List<ClinicOperatingHour>();
        }
    }


}

