using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using WeCare.Responsibles;
using WeCare.Tratamentos;

namespace WeCare.Patients
{
    public class Patient : AuditedAggregateRoot<Guid>
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(256)]
        public string? Address { get; set; }

        [MaxLength(40)]
        public string? Diag { get; set; }


        public Guid PrincipalResponsibleId { get; set; }
        public Responsible PrincipalResponsible { get; set; }

        public ICollection<Tratamento> Tratamentos { get; set; }
    }
}