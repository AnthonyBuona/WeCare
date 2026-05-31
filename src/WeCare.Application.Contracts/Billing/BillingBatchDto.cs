using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Billing
{
    public class BillingBatchDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExportDate { get; set; }
        public string XmlPayload { get; set; }
        public string HashSignature { get; set; }
    }
}
