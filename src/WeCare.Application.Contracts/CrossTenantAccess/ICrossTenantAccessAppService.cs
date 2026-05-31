using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WeCare.CrossTenantAccess
{
    public interface ICrossTenantAccessAppService : IApplicationService
    {
        Task<CrossTenantAccessConsentDto> CreateConsentAsync(CreateCrossTenantAccessConsentDto input);
        
        Task<CrossTenantAccessConsentDto> VerifyConsentAsync(VerifyConsentTokenDto input);
        
        Task<List<SharedAccessAuditLogDto>> GetAuditLogsAsync(Guid consentId);

        Task<string> EncryptMedicalNoteAsync(Guid consentId, string rawNote, string rawToken);

        Task<string> DecryptMedicalNoteAsync(Guid consentId, string encryptedNote, string rawToken);
    }
}
