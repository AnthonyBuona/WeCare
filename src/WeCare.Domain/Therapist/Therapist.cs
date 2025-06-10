using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
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

        // --- ADICIONE ESTA LINHA ---
        // Guarda a referência ao usuário do Identity
        public Guid UserId { get; set; }

        public ICollection<Tratamento> Tratamentos { get; set; }

        public Therapist()
        {
            Tratamentos = new HashSet<Tratamento>();
        }
    }
}