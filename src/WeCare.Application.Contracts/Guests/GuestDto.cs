using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace WeCare.Guests
{
    public class GuestDto : EntityDto<Guid>
    {
        public Guid ResponsibleId { get; set; }
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid? UserId { get; set; }
    }

    public class CreateGuestDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }
    }
}
