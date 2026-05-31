using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using WeCare.Permissions;

namespace WeCare.CrossTenantAccess
{
    public class CrossTenantAccessAppService : ApplicationService, ICrossTenantAccessAppService
    {
        private readonly IRepository<CrossTenantAccessConsent, Guid> _consentRepository;
        private readonly IRepository<SharedAccessAuditLog, Guid> _auditLogRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CrossTenantAccessAppService(
            IRepository<CrossTenantAccessConsent, Guid> consentRepository,
            IRepository<SharedAccessAuditLog, Guid> auditLogRepository,
            IDataFilter dataFilter,
            IHttpContextAccessor httpContextAccessor)
        {
            _consentRepository = consentRepository;
            _auditLogRepository = auditLogRepository;
            _dataFilter = dataFilter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CrossTenantAccessConsentDto> CreateConsentAsync(CreateCrossTenantAccessConsentDto input)
        {
            // Enforce permission guard
            await CheckPolicyAsync(WeCarePermissions.CrossTenantAccess.Create);

            // Generate raw token
            string rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            string tokenHash = ComputeSha256Hash(rawToken);

            var expirationDate = Clock.Now.AddDays(input.ExpirationDays);

            // Create consent entity
            var consent = new CrossTenantAccessConsent(
                GuidGenerator.Create(),
                input.PatientId,
                CurrentTenant.Id ?? Guid.Empty, // SourceTenantId is the current tenant
                input.TargetTenantId,
                expirationDate,
                input.GrantedPermissions,
                tokenHash,
                isRevoked: false,
                tenantId: CurrentTenant.Id
            );

            await _consentRepository.InsertAsync(consent);

            var dto = ObjectMapper.Map<CrossTenantAccessConsent, CrossTenantAccessConsentDto>(consent);
            dto.RawToken = rawToken; // Return the raw token ONLY once upon creation

            return dto;
        }

        public async Task<CrossTenantAccessConsentDto> VerifyConsentAsync(VerifyConsentTokenDto input)
        {
            await CheckPolicyAsync(WeCarePermissions.CrossTenantAccess.Verify);

            if (string.IsNullOrWhiteSpace(input.RawToken))
            {
                throw new UserFriendlyException("Token de consentimento inválido.");
            }

            string hashToSearch = ComputeSha256Hash(input.RawToken);
            CrossTenantAccessConsent consent = null;

            // Bypass multi-tenancy filters for scanning cross-tenant consents
            using (CurrentTenant.Change(null))
            using (_dataFilter.Disable<IMultiTenant>())
            {
                consent = await _consentRepository.FirstOrDefaultAsync(x => x.AuthTokenHash == hashToSearch);
            }

            if (consent == null)
            {
                throw new UserFriendlyException("Consentimento não encontrado ou token inválido.");
            }

            if (consent.IsRevoked)
            {
                throw new UserFriendlyException("Este consentimento foi revogado pelo responsável legal.");
            }

            if (consent.ExpirationDate < Clock.Now)
            {
                throw new UserFriendlyException("Este token de consentimento expirou.");
            }

            // Write immutable access audit log
            var clientIp = GetClientIpAddress();
            var auditLog = new SharedAccessAuditLog(
                GuidGenerator.Create(),
                consent.Id,
                CurrentUser.Id ?? Guid.Empty,
                "VerifyConsent",
                Clock.Now,
                clientIp,
                consent.TenantId
            );

            using (CurrentTenant.Change(consent.TenantId))
            {
                await _auditLogRepository.InsertAsync(auditLog);
            }

            return ObjectMapper.Map<CrossTenantAccessConsent, CrossTenantAccessConsentDto>(consent);
        }

        public async Task<List<SharedAccessAuditLogDto>> GetAuditLogsAsync(Guid consentId)
        {
            await CheckPolicyAsync(WeCarePermissions.CrossTenantAccess.ViewAuditLogs);

            var logs = await _auditLogRepository.GetListAsync(x => x.ConsentId == consentId);
            return ObjectMapper.Map<List<SharedAccessAuditLog>, List<SharedAccessAuditLogDto>>(logs);
        }

        public async Task<string> EncryptMedicalNoteAsync(Guid consentId, string rawNote, string rawToken)
        {
            // Verify consent and obtain details
            string hashToSearch = ComputeSha256Hash(rawToken);
            CrossTenantAccessConsent consent = null;

            using (CurrentTenant.Change(null))
            using (_dataFilter.Disable<IMultiTenant>())
            {
                consent = await _consentRepository.FirstOrDefaultAsync(x => x.AuthTokenHash == hashToSearch && x.Id == consentId);
            }

            if (consent == null || consent.IsRevoked || consent.ExpirationDate < Clock.Now)
            {
                throw new UserFriendlyException("Consentimento inválido ou expirado.");
            }

            // Encrypt
            var encrypted = EncryptAes256(rawNote, rawToken);

            // Audit
            var clientIp = GetClientIpAddress();
            var auditLog = new SharedAccessAuditLog(
                GuidGenerator.Create(),
                consent.Id,
                CurrentUser.Id ?? Guid.Empty,
                "EncryptNote",
                Clock.Now,
                clientIp,
                consent.TenantId
            );

            using (CurrentTenant.Change(consent.TenantId))
            {
                await _auditLogRepository.InsertAsync(auditLog);
            }

            return encrypted;
        }

        public async Task<string> DecryptMedicalNoteAsync(Guid consentId, string encryptedNote, string rawToken)
        {
            // Verify consent and obtain details
            string hashToSearch = ComputeSha256Hash(rawToken);
            CrossTenantAccessConsent consent = null;

            using (CurrentTenant.Change(null))
            using (_dataFilter.Disable<IMultiTenant>())
            {
                consent = await _consentRepository.FirstOrDefaultAsync(x => x.AuthTokenHash == hashToSearch && x.Id == consentId);
            }

            if (consent == null || consent.IsRevoked || consent.ExpirationDate < Clock.Now)
            {
                throw new UserFriendlyException("Consentimento inválido ou expirado.");
            }

            // Decrypt
            var decrypted = DecryptAes256(encryptedNote, rawToken);

            // Audit
            var clientIp = GetClientIpAddress();
            var auditLog = new SharedAccessAuditLog(
                GuidGenerator.Create(),
                consent.Id,
                CurrentUser.Id ?? Guid.Empty,
                "DecryptNote",
                Clock.Now,
                clientIp,
                consent.TenantId
            );

            using (CurrentTenant.Change(consent.TenantId))
            {
                await _auditLogRepository.InsertAsync(auditLog);
            }

            return decrypted;
        }

        #region Helper Methods

        private static string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static string EncryptAes256(string plainText, string rawToken)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                using (var sha256 = SHA256.Create())
                {
                    aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
                }
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    var iv = Convert.ToBase64String(aes.IV);
                    var cipher = Convert.ToBase64String(ms.ToArray());
                    return $"{iv}:{cipher}";
                }
            }
        }

        private static string DecryptAes256(string encryptedText, string rawToken)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

            var parts = encryptedText.Split(':');
            if (parts.Length != 2)
            {
                return encryptedText; // Fallback or throw exception if not in formatted state
            }

            var iv = Convert.FromBase64String(parts[0]);
            var cipher = Convert.FromBase64String(parts[1]);

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                using (var sha256 = SHA256.Create())
                {
                    aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
                }
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipher))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                return string.IsNullOrWhiteSpace(ip) ? "127.0.0.1" : ip;
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        #endregion
    }
}
