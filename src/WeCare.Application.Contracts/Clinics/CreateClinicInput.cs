using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;

namespace WeCare.Clinics;

public class CreateClinicInput
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string AdminEmailAddress { get; set; }

    [Required]
    [StringLength(128)]
    [DisableAuditing]
    public string AdminPassword { get; set; }

    [StringLength(18)] // CNPJ: 00.000.000/0000-00
    public string? CNPJ { get; set; }

    [StringLength(256)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Specializations { get; set; }
}
