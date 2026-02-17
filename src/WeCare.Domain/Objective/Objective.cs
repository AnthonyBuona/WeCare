using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WeCare.Patients;
using WeCare.Consultations;
using WeCare.Trainings;

namespace WeCare.Objectives
{
    public class Objective : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public Guid TherapistId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } // Ex: "Ativo", "Concluído"
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<Consultation> Consultations { get; set; }
        public virtual ICollection<Training> Trainings { get; protected set; } // Adicione esta linha

        public Objective()
        {
            Consultations = new HashSet<Consultation>();
            Trainings = new HashSet<Training>(); 
        }
    }
}