using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.TenantManagement;
using WeCare.Tratamentos;

namespace WeCare.Therapists
{
    public class Therapist : AuditedAggregateRoot<Guid>
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public Guid UserId { get; set; }

        public ICollection<Tratamento> Tratamentos { get; set; }

        public int TenantId { get; set; } // Chave estrangeira para o Tenant
        public Tenant Tenant { get; set; }

        public Therapist()
        {
            Tratamentos = new HashSet<Tratamento>();
        }
    }
}