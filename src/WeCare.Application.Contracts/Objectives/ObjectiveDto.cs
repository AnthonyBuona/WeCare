
using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Objectives
{
    public class ObjectiveDto : AuditedEntityDto<Guid>
    {
        public Guid PatientId { get; set; }
        public Guid TherapistId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}