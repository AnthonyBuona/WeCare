#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.Attendances
{
    public class Attendance : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid PatientId { get; set; }

        public DateTime SessionDate { get; set; }

        public AttendanceStatus Status { get; set; }

        public string Notes { get; set; }

        protected Attendance()
        {
        }

        public Attendance(
            Guid id,
            Guid patientId,
            DateTime sessionDate,
            AttendanceStatus status,
            string notes,
            Guid? tenantId = null)
            : base(id)
        {
            PatientId = patientId;
            SessionDate = sessionDate;
            Status = status;
            Notes = notes;
            TenantId = tenantId;
        }
    }
}
