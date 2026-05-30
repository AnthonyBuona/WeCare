#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.PeriodicReports
{
    public class PeriodicReport : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid PatientId { get; set; }

        public Guid TherapistId { get; set; }

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

        protected PeriodicReport()
        {
        }

        public PeriodicReport(
            Guid id,
            Guid patientId,
            Guid therapistId,
            string resumoClinico,
            string objetivosStatus,
            string engajamentoCasa,
            string proximosPassos,
            PeriodicReportStatus status,
            Guid? tenantId)
            : base(id)
        {
            PatientId = patientId;
            TherapistId = therapistId;
            ResumoClinico = resumoClinico;
            ObjetivosStatus = objetivosStatus;
            EngajamentoCasa = engajamentoCasa;
            ProximosPassos = proximosPassos;
            Status = status;
            TenantId = tenantId;
            Title = "Relatório Evolutivo";
            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;
        }

        public PeriodicReport(
            Guid id,
            Guid patientId,
            Guid therapistId,
            string title,
            DateTime startDate,
            DateTime endDate,
            string resumoClinico,
            string objetivosStatus,
            string engajamentoCasa,
            string proximosPassos,
            PeriodicReportStatus status,
            DateTime? responsibleSignatureDate = null,
            string? responsibleSignatureIP = null,
            string? responsibleSignatureCPF = null,
            string? parentSignatureHash = null,
            Guid? tenantId = null)
            : base(id)
        {
            PatientId = patientId;
            TherapistId = therapistId;
            Title = title;
            StartDate = startDate;
            EndDate = endDate;
            ResumoClinico = resumoClinico;
            ObjetivosStatus = objetivosStatus;
            EngajamentoCasa = engajamentoCasa;
            ProximosPassos = proximosPassos;
            Status = status;
            ResponsibleSignatureDate = responsibleSignatureDate;
            ResponsibleSignatureIP = responsibleSignatureIP;
            ResponsibleSignatureCPF = responsibleSignatureCPF;
            ParentSignatureHash = parentSignatureHash;
            TenantId = tenantId;
        }
    }
}
