using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace WeCare.Tratamentos
{
    public class TratamentoDto : AuditedEntityDto<Guid>
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }

        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; }

        public string Tipo { get; set; }
    }
}
