#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.Billing
{
    public class BillingBatch : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public string BatchNumber { get; set; }

        public DateTime ExportDate { get; set; }

        public string XmlPayload { get; set; }

        public string HashSignature { get; set; }

        protected BillingBatch()
        {
        }

        public BillingBatch(
            Guid id,
            string batchNumber,
            DateTime exportDate,
            string xmlPayload,
            string hashSignature,
            Guid? tenantId = null)
            : base(id)
        {
            BatchNumber = batchNumber;
            ExportDate = exportDate;
            XmlPayload = xmlPayload;
            HashSignature = hashSignature;
            TenantId = tenantId;
        }
    }
}
