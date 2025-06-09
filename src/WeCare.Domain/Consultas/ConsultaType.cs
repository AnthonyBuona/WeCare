using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Therapists;

namespace WeCare.Consultas
{
    public class ConsultaType : AuditedAggregateRoot<Guid>
    {

        public Patient Patient { get; set; }

        public Therapist Therapist { get; set; }

        public string TipoConsulta { get; set; }

    }
}
