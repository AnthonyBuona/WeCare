using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace WeCare.Clinics
{
    public class ClinicOperatingHour : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid ClinicId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStart { get; set; }
        public TimeSpan? BreakEnd { get; set; }
        public bool IsClosed { get; set; }

        protected ClinicOperatingHour() { }

        public ClinicOperatingHour(Guid id, Guid clinicId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool isClosed = false) 
            : base(id)
        {
            ClinicId = clinicId;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            IsClosed = isClosed;
        }
    }
}
