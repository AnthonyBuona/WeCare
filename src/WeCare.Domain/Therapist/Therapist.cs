using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Tratamentos;
using WeCare.Patients;

namespace WeCare.Therapists
{
    public class Therapist : AuditedAggregateRoot<Guid>
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public IList<Patient> Patients { get; set; } = new List<Patient>();
        public ICollection<Tratamento> Tratamentos { get; set; }
    }
}

