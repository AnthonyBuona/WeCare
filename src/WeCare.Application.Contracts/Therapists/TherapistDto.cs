using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Therapists
{
    public class TherapistDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
    }
}