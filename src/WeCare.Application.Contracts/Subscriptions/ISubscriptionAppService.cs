using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WeCare.Subscriptions
{
    public class RegisterTrialInputDto
    {
        [Required]
        [MaxLength(128)]
        public string ClinicName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string AdminEmailAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string AdminPassword { get; set; } = string.Empty;

        [MaxLength(18)]
        public string? CNPJ { get; set; }

        [Required]
        [MaxLength(50)]
        public string SelectedPlan { get; set; } = "Prata"; // Bronze, Prata, Ouro
    }

    public class SubscriptionStatusDto
    {
        public bool IsActive { get; set; }
        public string PlanName { get; set; } = "Bronze";
        public string Status { get; set; } = "Nenhum";
        public DateTime? TrialEndDate { get; set; }
        public string StripeCustomerId { get; set; } = string.Empty;
    }

    public interface ISubscriptionAppService : IApplicationService
    {
        Task RegisterTrialAsync(RegisterTrialInputDto input);
        Task<SubscriptionStatusDto> GetStatusAsync();
        Task UpdateSubscriptionPlanAsync(string planName);
    }
}
