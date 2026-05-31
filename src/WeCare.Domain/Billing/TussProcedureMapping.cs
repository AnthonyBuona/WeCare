#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace WeCare.Billing
{
    public class TussProcedureMapping : FullAuditedAggregateRoot<Guid>
    {
        public string Specialty { get; set; }

        public string TussCode { get; set; }

        public string Description { get; set; }

        protected TussProcedureMapping()
        {
        }

        public TussProcedureMapping(
            Guid id,
            string specialty,
            string tussCode,
            string description)
            : base(id)
        {
            Specialty = specialty;
            TussCode = tussCode;
            Description = description;
        }
    }
}
