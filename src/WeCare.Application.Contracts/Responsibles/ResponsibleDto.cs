using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace WeCare.Responsibles;

public class ResponsibleDto : AuditedEntityDto<Guid>
{

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

}
