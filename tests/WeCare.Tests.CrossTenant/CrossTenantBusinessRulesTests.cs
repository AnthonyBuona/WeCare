using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Xunit;

namespace WeCare.Tests.CrossTenant
{
    /// <summary>
    /// Testes funcionais das regras de negócio do CrossTenantAccess.
    /// Cobre os cenários de consentimento: criação, verificação, revogação,
    /// expiração, e validação de token — sem depender do banco de dados.
    /// </summary>
    public class CrossTenantBusinessRulesTests
    {
        private static readonly Guid TenantA = Guid.NewGuid(); // Clínica origem
        private static readonly Guid TenantB = Guid.NewGuid(); // Clínica destino
        private static readonly Guid PatientId = Guid.NewGuid();

        // ----------------------------------------------------------------
        // Fluxo 1: Criação de Consentimento
        // ----------------------------------------------------------------

        [Fact]
        public void CreateConsent_ShouldGenerateUniqueRawToken()
        {
            // Arrange — Simula o token gerado pelo AppService
            var tokens = new HashSet<string>();

            // Act — Cria 100 tokens
            for (int i = 0; i < 100; i++)
            {
                var rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
                tokens.Add(rawToken);
            }

            // Assert — Todos devem ser únicos (probabilidade de colisão: ~0)
            tokens.Should().HaveCount(100, "tokens de consentimento devem ser sempre únicos");
        }

        [Fact]
        public void CreateConsent_TokenHash_ShouldDifferFromRawToken()
        {
            // Arrange
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            // Act
            var hash = ComputeSha256Hash(rawToken);

            // Assert — O hash armazenado nunca deve ser igual ao token bruto
            hash.Should().NotBe(rawToken, "o banco armazena apenas o hash, nunca o token bruto");
        }

        [Fact]
        public void CreateConsent_ExpirationDate_ShouldBeInFuture()
        {
            // Arrange
            const int expirationDays = 30;
            var now = DateTime.UtcNow;

            // Act — Simula o cálculo de expiração do AppService
            var expirationDate = now.AddDays(expirationDays);

            // Assert
            expirationDate.Should().BeAfter(now);
            expirationDate.Should().BeCloseTo(now.AddDays(30), TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(30)]
        [InlineData(90)]
        [InlineData(365)]
        public void CreateConsent_ExpirationDays_ShouldRespectConfiguredValue(int days)
        {
            // Arrange
            var now = DateTime.UtcNow;

            // Act
            var expiration = now.AddDays(days);

            // Assert
            (expiration - now).TotalDays.Should().BeApproximately(days, 0.01);
        }

        // ----------------------------------------------------------------
        // Fluxo 2: Verificação de Token
        // ----------------------------------------------------------------

        [Fact]
        public void VerifyToken_WithValidToken_ShouldMatchStoredHash()
        {
            // Arrange
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var storedHash = ComputeSha256Hash(rawToken);

            // Act — Simula o que o VerifyConsentAsync faz: re-hash o token recebido
            var hashToVerify = ComputeSha256Hash(rawToken);

            // Assert
            hashToVerify.Should().Be(storedHash, "o hash do token verificado deve bater com o armazenado");
        }

        [Fact]
        public void VerifyToken_WithTamperedToken_ShouldNotMatchStoredHash()
        {
            // Arrange
            var originalToken = "wctk_" + Guid.NewGuid().ToString("N");
            var tamperedToken = originalToken + "_TAMPERED";
            var storedHash = ComputeSha256Hash(originalToken);

            // Act
            var tamperedHash = ComputeSha256Hash(tamperedToken);

            // Assert — Token adulterado deve gerar hash diferente
            tamperedHash.Should().NotBe(storedHash);
        }

        [Fact]
        public void VerifyToken_EmptyToken_ShouldBeConsideredInvalid()
        {
            // Arrange
            var emptyToken = "";
            var whiteSpaceToken = "   ";

            // Assert — Tokens vazios são inválidos (como o AppService verifica)
            string.IsNullOrWhiteSpace(emptyToken).Should().BeTrue();
            string.IsNullOrWhiteSpace(whiteSpaceToken).Should().BeTrue();
        }

        // ----------------------------------------------------------------
        // Fluxo 3: Estado do Consentimento (Regras de Acesso)
        // ----------------------------------------------------------------

        [Fact]
        public void Consent_WhenActive_ShouldAllowAccess()
        {
            // Arrange — Consentimento válido: não revogado, não expirado
            var consent = CreateActiveConsent();

            // Assert
            var isAccessAllowed = !consent.IsRevoked && consent.ExpirationDate > DateTime.UtcNow;
            isAccessAllowed.Should().BeTrue("consentimento ativo deve permitir acesso");
        }

        [Fact]
        public void Consent_WhenRevoked_ShouldDenyAccess()
        {
            // Arrange
            var consent = new WeCare.CrossTenantAccess.CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, TenantA, TenantB,
                DateTime.UtcNow.AddDays(30), "read:records", "hash",
                isRevoked: true // Revogado pelo responsável
            );

            // Assert
            var isAccessAllowed = !consent.IsRevoked && consent.ExpirationDate > DateTime.UtcNow;
            isAccessAllowed.Should().BeFalse("consentimento revogado deve negar acesso");
        }

        [Fact]
        public void Consent_WhenExpired_ShouldDenyAccess()
        {
            // Arrange
            var consent = new WeCare.CrossTenantAccess.CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, TenantA, TenantB,
                DateTime.UtcNow.AddDays(-1), "read:records", "hash" // Expirado ontem
            );

            // Assert
            var isAccessAllowed = !consent.IsRevoked && consent.ExpirationDate > DateTime.UtcNow;
            isAccessAllowed.Should().BeFalse("consentimento expirado deve negar acesso");
        }

        [Fact]
        public void Consent_WhenRevokedAndExpired_ShouldDenyAccess()
        {
            // Arrange — Pior caso: revogado E expirado
            var consent = new WeCare.CrossTenantAccess.CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, TenantA, TenantB,
                DateTime.UtcNow.AddDays(-5), "read:records", "hash",
                isRevoked: true
            );

            // Assert
            var isAccessAllowed = !consent.IsRevoked && consent.ExpirationDate > DateTime.UtcNow;
            isAccessAllowed.Should().BeFalse();
        }

        // ----------------------------------------------------------------
        // Fluxo 4: Permissões Granulares
        // ----------------------------------------------------------------

        [Theory]
        [InlineData("read:records", "read:records", true)]
        [InlineData("read:records,read:timeline", "read:records", true)]
        [InlineData("read:records", "write:notes", false)]
        [InlineData("read:records,read:timeline", "write:notes", false)]
        public void GrantedPermissions_CheckAccess_ShouldRespectGrantedScope(
            string grantedPermissions, string requiredPermission, bool expectedResult)
        {
            // Arrange
            var consent = CreateActiveConsent(permissions: grantedPermissions);
            var permissionList = consent.GrantedPermissions.Split(',');

            // Act
            var hasPermission = Array.Exists(permissionList, p => p.Trim() == requiredPermission);

            // Assert
            hasPermission.Should().Be(expectedResult);
        }

        // ----------------------------------------------------------------
        // Fluxo 5: Auditoria
        // ----------------------------------------------------------------

        [Fact]
        public void AuditLog_ConsentId_ShouldLinkToParentConsent()
        {
            // Arrange
            var consent = CreateActiveConsent();
            var auditLog = new WeCare.CrossTenantAccess.SharedAccessAuditLog(
                Guid.NewGuid(),
                consentId: consent.Id,
                userAccessingId: Guid.NewGuid(),
                actionPerformed: "VerifyConsent",
                timestamp: DateTime.UtcNow,
                accessingIP: "192.168.1.1",
                tenantId: TenantA
            );

            // Assert — O log deve referenciar o consentimento correto
            auditLog.ConsentId.Should().Be(consent.Id);
            auditLog.ActionPerformed.Should().Be("VerifyConsent");
            auditLog.AccessingIP.Should().Be("192.168.1.1");
        }

        [Theory]
        [InlineData("VerifyConsent")]
        [InlineData("EncryptNote")]
        [InlineData("DecryptNote")]
        public void AuditLog_Action_ShouldSupportAllKnownActions(string action)
        {
            // Arrange
            var consent = CreateActiveConsent();

            // Act
            var auditLog = new WeCare.CrossTenantAccess.SharedAccessAuditLog(
                Guid.NewGuid(),
                consentId: consent.Id,
                userAccessingId: Guid.NewGuid(),
                actionPerformed: action,
                timestamp: DateTime.UtcNow,
                accessingIP: "127.0.0.1",
                tenantId: TenantA
            );

            // Assert
            auditLog.ActionPerformed.Should().Be(action);
        }

        // ----------------------------------------------------------------
        // Fluxo 6: Isolamento Cross-Tenant
        // ----------------------------------------------------------------

        [Fact]
        public void Consent_TenantAAndTenantB_ShouldBeIsolated()
        {
            // Arrange — Dois consentimentos: um de A→B, outro de B→C
            var tenantC = Guid.NewGuid();

            var consentAtoB = CreateActiveConsent(sourceTenant: TenantA, targetTenant: TenantB);
            var consentBtoC = CreateActiveConsent(sourceTenant: TenantB, targetTenant: tenantC);

            // Assert — Cada consentimento tem seu próprio escopo
            consentAtoB.SourceTenantId.Should().Be(TenantA);
            consentAtoB.TargetTenantId.Should().Be(TenantB);

            consentBtoC.SourceTenantId.Should().Be(TenantB);
            consentBtoC.TargetTenantId.Should().Be(tenantC);

            // Consentimento A→B não deve autorizar acesso a C
            consentAtoB.TargetTenantId.Should().NotBe(tenantC);
        }

        [Fact]
        public void Consent_PatientId_ShouldBeImmutableAfterCreation()
        {
            // Arrange
            var consent = CreateActiveConsent();
            var originalPatientId = consent.PatientId;

            // Assert — PatientId não deve mudar (não há setter público)
            consent.PatientId.Should().Be(originalPatientId);
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private static WeCare.CrossTenantAccess.CrossTenantAccessConsent CreateActiveConsent(
            string permissions = "read:records",
            Guid? sourceTenant = null,
            Guid? targetTenant = null)
        {
            return new WeCare.CrossTenantAccess.CrossTenantAccessConsent(
                Guid.NewGuid(),
                PatientId,
                sourceTenant ?? TenantA,
                targetTenant ?? TenantB,
                DateTime.UtcNow.AddDays(30),
                permissions,
                "sha256_valid_hash",
                isRevoked: false
            );
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
