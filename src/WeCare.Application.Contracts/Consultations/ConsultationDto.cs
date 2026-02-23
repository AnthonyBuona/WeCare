using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;
using WeCare.Application.Contracts.PerformedTrainings;

namespace WeCare.Consultations
{
    public class ConsultationDto : AuditedEntityDto<Guid>
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; }
        public Guid TratamentoId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public string Specialty { get; set; }
        public string MainTraining { get; set; }
        public string Duration { get; set; }
        public ConsultationStatus Status { get; set; }
        public Guid? ObjectiveId { get; set; }
        public List<PerformedTrainingDto> PerformedTrainings { get; set; }
    }
}