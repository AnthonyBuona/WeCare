using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Patients;

namespace WeCare.Responsibles
{
    public class Responsible : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        // Construtor sem parâmetros (necessário para EF)
        public Responsible() { }

        // Construtor personalizado para definir o Id externamente
        public Responsible(Guid id)
        {
            Id = id; // aqui o protected set é acessível
        }

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

        public Guid? UserId { get; set; } // Opcional inicialmente para não quebrar dados existentes
        
        [Phone]
        [StringLength(20)]
        [Display(Name = "Telefone do Responsável")]
        public string? PhoneNumber { get; set; }

        public ICollection<Patient> Patients { get; set; } = new List<Patient>();

    }
}
