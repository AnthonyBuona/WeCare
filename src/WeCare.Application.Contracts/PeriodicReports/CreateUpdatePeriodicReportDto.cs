using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.PeriodicReports
{
    public class CreateUpdatePeriodicReportDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid TherapistId { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(2000)]
        public string ResumoClinico { get; set; }

        [StringLength(2000)]
        public string ObjetivosStatus { get; set; }

        [StringLength(2000)]
        public string EngajamentoCasa { get; set; }

        [StringLength(2000)]
        public string ProximosPassos { get; set; }

        [Required]
        public PeriodicReportStatus Status { get; set; }
    }
}
