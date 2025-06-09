using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Patients
{
    public class CreateUpdatePatientDto
    {
        [Required]
        [StringLength(128)]
        [Display(Name = "Nome do Paciente")]
        public string Name{ get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF do responsável é obrigatório")]
        [StringLength(11)]
        [Display(Name = "CPF do responsável")]
        public string CPF { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        [Display(Name = "Nome do Responsável")]
        public string NameResponsible { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        [Display(Name = "E-mail do Responsável")]
        public string EmailAddress { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        [Display(Name = "Telefone do Responsável")]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime BirthDate { get; set; }

        [StringLength(256)]
        [Display(Name = "Endereço")]
        public string? Address { get; set; }

        [StringLength(40)]
        [Display(Name = "Diagnóstico")]
        public string? Diag { get; set; }
    }
}
