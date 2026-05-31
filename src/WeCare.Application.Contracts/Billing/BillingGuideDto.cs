using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Billing
{
    public class BillingGuideDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid ConsultationId { get; set; }
        public string HealthInsuranceName { get; set; }
        public string CardNumber { get; set; }
        public string AuthorizationPassword { get; set; }
        public decimal ConsultationValue { get; set; }
        public string Status { get; set; }
    }
}
