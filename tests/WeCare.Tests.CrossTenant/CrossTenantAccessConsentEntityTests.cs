using System;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using WeCare.CrossTenantAccess;
using Xunit;

namespace WeCare.Tests.CrossTenant
{
    /// <summary>
    /// Testes unitários da entidade de domínio CrossTenantAccessConsent.
    /// Cobre a criação, estado, e invariantes da entidade.
    /// </summary>
    public class CrossTenantAccessConsentEntityTests
    {
        private static readonly Guid PatientId = Guid.NewGuid();
        private static readonly Guid SourceTenantId = Guid.NewGuid();
        private static readonly Guid TargetTenantId = Guid.NewGuid();

        // ----------------------------------------------------------------
        // Criação
        // ----------------------------------------------------------------

        [Fact]
        public void Constructor_WithValidData_ShouldCreateConsent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expiration = DateTime.UtcNow.AddDays(30);
            const string permissions = "read:records,read:timeline";
            const string tokenHash = "sha256hashvalue";

            // Act
            var consent = new CrossTenantAccessConsent(
                id, PatientId, SourceTenantId, TargetTenantId,
                expiration, permissions, tokenHash
            );

            // Assert
            consent.Id.Should().Be(id);
            consent.PatientId.Should().Be(PatientId);
            consent.SourceTenantId.Should().Be(SourceTenantId);
            consent.TargetTenantId.Should().Be(TargetTenantId);
            consent.ExpirationDate.Should().Be(expiration);
            consent.GrantedPermissions.Should().Be(permissions);
            consent.AuthTokenHash.Should().Be(tokenHash);
            consent.IsRevoked.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WhenTenantIdNotProvided_ShouldDefaultToSourceTenantId()
        {
            // Arrange & Act
            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                DateTime.UtcNow.AddDays(1), "read:records", "hash",
                isRevoked: false,
                tenantId: null // não fornecido
            );

            // Assert — TenantId deve ser SourceTenantId (regra de multitenancy)
            consent.TenantId.Should().Be(SourceTenantId);
        }

        [Fact]
        public void Constructor_WhenExplicitTenantIdProvided_ShouldUseProvidedTenantId()
        {
            // Arrange
            var explicitTenantId = Guid.NewGuid();

            // Act
            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                DateTime.UtcNow.AddDays(1), "read:records", "hash",
                isRevoked: false,
                tenantId: explicitTenantId
            );

            // Assert
            consent.TenantId.Should().Be(explicitTenantId);
        }

        [Fact]
        public void Constructor_WithIsRevoked_ShouldMarkConsentAsRevoked()
        {
            // Arrange & Act
            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                DateTime.UtcNow.AddDays(1), "read:records", "hash",
                isRevoked: true
            );

            // Assert
            consent.IsRevoked.Should().BeTrue();
        }

        // ----------------------------------------------------------------
        // Invariantes de negócio
        // ----------------------------------------------------------------

        [Fact]
        public void Consent_WhenExpired_ShouldHaveExpirationDateInPast()
        {
            // Arrange
            var expiredDate = DateTime.UtcNow.AddDays(-1);

            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                expiredDate, "read:records", "hash"
            );

            // Assert
            var isExpired = consent.ExpirationDate < DateTime.UtcNow;
            isExpired.Should().BeTrue("consentimento com ExpirationDate no passado deve ser considerado expirado");
        }

        [Fact]
        public void Consent_WhenActive_ShouldHaveExpirationDateInFuture()
        {
            // Arrange
            var futureDate = DateTime.UtcNow.AddDays(30);

            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                futureDate, "read:records", "hash"
            );

            // Assert
            var isExpired = consent.ExpirationDate < DateTime.UtcNow;
            isExpired.Should().BeFalse("consentimento com ExpirationDate no futuro deve ser válido");
        }

        [Theory]
        [InlineData("read:records")]
        [InlineData("read:records,read:timeline")]
        [InlineData("read:records,read:timeline,write:notes")]
        public void Consent_GrantedPermissions_ShouldPreserveAllPermissions(string permissions)
        {
            // Arrange & Act
            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                DateTime.UtcNow.AddDays(1), permissions, "hash"
            );

            // Assert
            consent.GrantedPermissions.Should().Be(permissions);
        }

        [Fact]
        public void Consent_SourceAndTargetTenant_ShouldBeDifferent()
        {
            // Arrange
            var consent = new CrossTenantAccessConsent(
                Guid.NewGuid(), PatientId, SourceTenantId, TargetTenantId,
                DateTime.UtcNow.AddDays(1), "read:records", "hash"
            );

            // Assert — Clinicas fonte e destino devem ser sempre diferentes
            consent.SourceTenantId.Should().NotBe(consent.TargetTenantId);
        }
    }
}
