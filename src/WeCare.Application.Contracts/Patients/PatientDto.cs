using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace WeCare.Patients;

public class PatientDto : AuditedEntityDto<Guid>
{
   
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    public DateTime BirthDate { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }


    [MaxLength(256)]
    public string? Address { get; set; }


    [MaxLength(40)]
    public string? Diag { get; set; }
    
}
