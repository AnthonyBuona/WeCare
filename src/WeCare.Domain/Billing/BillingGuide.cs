#pragma warning disable CS8618
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WeCare.Billing
{
    public class BillingGuide : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid ConsultationId { get; set; }

        public string HealthInsuranceName { get; set; }

        public string CardNumber { get; set; }

        public string AuthorizationPassword { get; set; }

        public decimal ConsultationValue { get; set; }

        public string Status { get; set; }

        protected BillingGuide()
        {
        }

        public BillingGuide(
            Guid id,
            Guid consultationId,
            string healthInsuranceName,
            string cardNumber,
            string authorizationPassword,
            decimal consultationValue,
            string status = "Pending",
            Guid? tenantId = null)
            : base(id)
        {
            ConsultationId = consultationId;
            HealthInsuranceName = healthInsuranceName;
            CardNumber = cardNumber;
            AuthorizationPassword = authorizationPassword;
            ConsultationValue = consultationValue;
            Status = status;
            TenantId = tenantId;
        }
    }
}
