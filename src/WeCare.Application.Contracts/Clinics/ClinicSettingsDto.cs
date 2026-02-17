using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Clinics
{
    public class ClinicSettingsDto
    {
        // White-Label
        [MaxLength(500)]
        public string? LogoUrl { get; set; }
        
        [MaxLength(10)]
        public string? PrimaryColor { get; set; }
        
        [MaxLength(10)]
        public string? SecondaryColor { get; set; }

        // Address & Contact
        [MaxLength(256)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; }

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
        public int AppointmentDurationMinutes { get; set; }

        public List<ClinicOperatingHourDto> OperatingHours { get; set; } = new();
    }
}
