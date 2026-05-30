using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.PeriodicReports
{
    public class PeriodicReportDto : FullAuditedEntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ResumoClinico { get; set; }
        public string ObjetivosStatus { get; set; }
        public string EngajamentoCasa { get; set; }
        public string ProximosPassos { get; set; }
        public PeriodicReportStatus Status { get; set; }
        public DateTime? ResponsibleSignatureDate { get; set; }
        public string? ResponsibleSignatureIP { get; set; }
        public string? ResponsibleSignatureCPF { get; set; }
        public string? ParentSignatureHash { get; set; }
    }
}
