using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using WeCare.Clinics;

namespace WeCare.Subscriptions
{
    public class SubscriptionAppService : ApplicationService, ISubscriptionAppService
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IRepository<Clinic, Guid> _clinicRepository;
        private readonly IDataFilter _dataFilter;
        private readonly ICurrentTenant _currentTenant;

        public SubscriptionAppService(
            ITenantAppService tenantAppService,
            ITenantRepository tenantRepository,
            IRepository<Clinic, Guid> clinicRepository,
            IDataFilter dataFilter,
            ICurrentTenant currentTenant)
        {
            _tenantAppService = tenantAppService;
            _tenantRepository = tenantRepository;
            _clinicRepository = clinicRepository;
            _dataFilter = dataFilter;
            _currentTenant = currentTenant;
        }

        [AllowAnonymous]
        public async Task RegisterTrialAsync(RegisterTrialInputDto input)
        {
            if (_currentTenant.Id != null)
            {
                throw new UserFriendlyException("Registro de trial só pode ser feito a partir da página pública (Host).");
            }

            // 1. Create the Tenant via TenantManagement
            var tenantCreateInput = new TenantCreateDto
            {
                Name = input.ClinicName,
                AdminEmailAddress = input.AdminEmailAddress,
                AdminPassword = input.AdminPassword
            };

            var tenantDto = await _tenantAppService.CreateAsync(tenantCreateInput);

            // 2. Load the created tenant to update SaaS extra properties
            var tenant = await _tenantRepository.GetAsync(tenantDto.Id);
            
            // Set administrative credentials & subscription details
            tenant.SetProperty("AdminEmail", input.AdminEmailAddress);
            tenant.SetProperty("AdminPassword", input.AdminPassword);
            tenant.SetProperty("PlanName", input.SelectedPlan);
            tenant.SetProperty("SubscriptionStatus", "ActiveTrial");
            tenant.SetProperty("TrialEndDate", DateTime.UtcNow.AddDays(14));
            tenant.SetProperty("CNPJ", input.CNPJ ?? "");

            await _tenantRepository.UpdateAsync(tenant, autoSave: true);

            // 3. Create the corresponding Clinic entity mapped to this Tenant
            var clinic = new Clinic
            {
                TenantId = tenantDto.Id,
                Name = input.ClinicName,
                CNPJ = input.CNPJ,
                Email = input.AdminEmailAddress,
                Status = ClinicStatus.Active
            };

            // Disable MultiTenant filter to save a tenant entity from the anonymous host context
            using (_dataFilter.Disable<IMultiTenant>())
            {
                await _clinicRepository.InsertAsync(clinic, autoSave: true);
            }
        }

        [Authorize]
        public async Task<SubscriptionStatusDto> GetStatusAsync()
        {
            if (_currentTenant.Id == null)
            {
                return new SubscriptionStatusDto { IsActive = false, Status = "Host Context" };
            }

            var tenant = await _tenantRepository.GetAsync(_currentTenant.Id.Value);

            var planName = tenant.GetProperty<string>("PlanName") ?? "Bronze";
            var status = tenant.GetProperty<string>("SubscriptionStatus") ?? "ActiveTrial";
            var trialEndDate = tenant.GetProperty<DateTime?>("TrialEndDate");
            var stripeCustomerId = tenant.GetProperty<string>("StripeCustomerId") ?? "";

            bool isActive = status == "Active" || status == "ActiveTrial" || (trialEndDate.HasValue && trialEndDate.Value > DateTime.UtcNow);

            return new SubscriptionStatusDto
            {
                IsActive = isActive,
                PlanName = planName,
                Status = status,
                TrialEndDate = trialEndDate,
                StripeCustomerId = stripeCustomerId
            };
        }

        [Authorize]
        public async Task UpdateSubscriptionPlanAsync(string planName)
        {
            if (_currentTenant.Id == null)
            {
                throw new UserFriendlyException("Essa funcionalidade só está disponível para clínicas configuradas (Multi-tenant).");
            }

            var tenant = await _tenantRepository.GetAsync(_currentTenant.Id.Value);
            tenant.SetProperty("PlanName", planName);
            
            // Simulates upgrading a subscription and putting it active
            tenant.SetProperty("SubscriptionStatus", "Active");
            tenant.SetProperty("TrialEndDate", (DateTime?)null);

            await _tenantRepository.UpdateAsync(tenant, autoSave: true);
        }
    }
}
