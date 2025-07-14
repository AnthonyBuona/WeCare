using System.ComponentModel.DataAnnotations;

namespace WeCare.Therapists
{
    public class CreateUpdateTherapistDto
    {
        [Required]
        [StringLength(128)]
        [Display(Name = "Nome Completo")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}