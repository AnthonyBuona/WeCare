﻿using System;
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

        [MaxLength(100)]
        public string Specialization { get; set; }
        public ICollection<Tratamento> Tratamentos { get; set; }

        public Guid? TenantId { get; set; } 

        public Therapist()
        {
            Tratamentos = new HashSet<Tratamento>();
        }
    }
}