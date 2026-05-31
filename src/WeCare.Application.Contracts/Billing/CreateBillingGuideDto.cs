using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Billing
{
    public class CreateBillingGuideDto
    {
        [Required]
        public Guid ConsultationId { get; set; }

        [Required]
        [StringLength(128)]
        public string HealthInsuranceName { get; set; }

        [Required]
        [StringLength(50)]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string AuthorizationPassword { get; set; }

        [Required]
        public decimal ConsultationValue { get; set; }
    }
}
