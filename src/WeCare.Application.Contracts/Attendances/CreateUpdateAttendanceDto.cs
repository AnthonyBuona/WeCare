using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Attendances
{
    public class CreateUpdateAttendanceDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
