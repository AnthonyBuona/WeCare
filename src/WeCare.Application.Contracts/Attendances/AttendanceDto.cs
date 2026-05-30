using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Attendances
{
    public class AttendanceDto : FullAuditedEntityDto<Guid>
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime SessionDate { get; set; }
        public AttendanceStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
