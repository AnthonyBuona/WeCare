using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Guests
{
    public class CreateUpdateGuestDto
    {
        public Guid? ResponsibleId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Relationship { get; set; }

        // We will create a user for them, so we need a password? 
        // Or generate one? Let's ask to provide one for now, like Therapists.
        [Required]
        [StringLength(128)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string UserName { get; set; } = string.Empty;
    }
}
