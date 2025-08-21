using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Consultations;

namespace WeCare.Objectives
{
    public class Objective : AuditedAggregateRoot<Guid>
    {
        public Guid PatientId { get; set; }
        public Guid TherapistId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } // Ex: "Ativo", "Concluído"
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<Consultation> Consultations { get; set; }
    }
}