using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WeCare.Billing
{
    public interface IBillingAppService : IApplicationService
    {
        Task<TussProcedureMappingDto> CreateTussMappingAsync(string specialty, string tussCode, string description);
        
        Task<List<TussProcedureMappingDto>> GetTussMappingsAsync();
        
        Task<BillingGuideDto> CreateBillingGuideAsync(CreateBillingGuideDto input);
        
        Task<List<BillingGuideDto>> GetPendingGuidesAsync();
        
        Task<BillingBatchDto> GenerateBillingBatchAsync(List<Guid> guideIds, string base64PfxCertificate, string pfxPassword);
    }
}
