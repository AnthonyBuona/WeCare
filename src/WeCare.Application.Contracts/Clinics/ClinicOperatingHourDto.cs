using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Clinics
{
    public class ClinicOperatingHourDto : EntityDto<Guid>
    {
        public Guid ClinicId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStart { get; set; }
        public TimeSpan? BreakEnd { get; set; }
        public bool IsClosed { get; set; }
    }
}
