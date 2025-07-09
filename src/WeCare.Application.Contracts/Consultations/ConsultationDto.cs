using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Consultations
{
    public class ConsultationDto : AuditedEntityDto<Guid>
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
    }
}