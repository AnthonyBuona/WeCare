using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace WeCare.Consultas;

public class ConsultaTypeDto : AuditedEntityDto<Guid>
{

    public Guid PatientId { get; set; }
    public Guid TherapistId { get; set; }
    public string TipoConsulta { get; set; }

}
