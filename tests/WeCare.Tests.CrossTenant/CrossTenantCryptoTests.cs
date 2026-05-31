using System;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Xunit;

namespace WeCare.Tests.CrossTenant
{
    /// <summary>
    /// Testes unitários da lógica criptográfica do CrossTenantAccessAppService.
    /// Testa SHA-256 token hashing, AES-256 encrypt/decrypt, e invariantes do token.
    /// Como esses são métodos privados/static, testamos a lógica equivalente
    /// de forma white-box para garantir que a implementação está correta.
    /// </summary>
    public class CrossTenantCryptoTests
    {
        // ----------------------------------------------------------------
        // SHA-256 Token Hashing
        // ----------------------------------------------------------------

        [Fact]
        public void ComputeSha256Hash_SameInput_ShouldProduceSameHash()
        {
            // Arrange
            var token = "wctk_abc123xyz";

            // Act
            var hash1 = ComputeSha256Hash(token);
            var hash2 = ComputeSha256Hash(token);

            // Assert — Hash é determinístico
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void ComputeSha256Hash_DifferentInputs_ShouldProduceDifferentHashes()
        {
            // Arrange
            var token1 = "wctk_" + Guid.NewGuid().ToString("N");
            var token2 = "wctk_" + Guid.NewGuid().ToString("N");

            // Act
            var hash1 = ComputeSha256Hash(token1);
            var hash2 = ComputeSha256Hash(token2);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void ComputeSha256Hash_Output_ShouldBe64HexChars()
        {
            // Arrange
            var token = "wctk_test_token_value";

            // Act
            var hash = ComputeSha256Hash(token);

            // Assert — SHA-256 = 32 bytes = 64 hex chars
            hash.Should().HaveLength(64);
            hash.Should().MatchRegex("^[0-9a-f]{64}$", "SHA-256 deve ser hexadecimal lowercase");
        }

        [Fact]
        public void TokenFormat_WctkPrefix_ShouldBePresent()
        {
            // Arrange — Simula a geração de token como o AppService faz
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            // Assert — Token deve ter prefixo wctk_ e comprimento de 5 + 32 + 32 = 69 chars
            rawToken.Should().StartWith("wctk_");
            rawToken.Length.Should().Be(5 + 32 + 32); // "wctk_" + 2x GUID sem hífens
        }

        // ----------------------------------------------------------------
        // AES-256 Encrypt / Decrypt Round-Trip
        // ----------------------------------------------------------------

        [Fact]
        public void EncryptDecrypt_RoundTrip_ShouldReturnOriginalText()
        {
            // Arrange
            var rawNote = "Paciente demonstra melhoras significativas nas sessões de ABA. Próxima consulta em 15 dias.";
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            // Act
            var encrypted = EncryptAes256(rawNote, rawToken);
            var decrypted = DecryptAes256(encrypted, rawToken);

            // Assert
            decrypted.Should().Be(rawNote);
        }

        [Fact]
        public void Encrypt_SameTextSameToken_ShouldProduceDifferentCiphertexts()
        {
            // Arrange — AES usa IV aleatório por chamada (deve ser não-determinístico)
            var rawNote = "Nota clínica para teste.";
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N");

            // Act
            var encrypted1 = EncryptAes256(rawNote, rawToken);
            var encrypted2 = EncryptAes256(rawNote, rawToken);

            // Assert — IV diferente a cada chamada = ciphertexts distintos (segurança)
            encrypted1.Should().NotBe(encrypted2, "AES com IV aleatório não deve ser determinístico");
        }

        [Fact]
        public void Encrypt_OutputFormat_ShouldContainIvAndCipherSeparatedByColon()
        {
            // Arrange
            var rawNote = "Nota de teste";
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N");

            // Act
            var encrypted = EncryptAes256(rawNote, rawToken);

            // Assert — Formato: "{base64_iv}:{base64_cipher}"
            encrypted.Should().Contain(":", "formato deve ser IV:CipherText");
            var parts = encrypted.Split(':');
            parts.Should().HaveCount(2);
            // IV base64 deve ser decodificável
            var ivBytes = Convert.FromBase64String(parts[0]);
            ivBytes.Should().HaveCount(16, "AES IV deve ter 16 bytes");
        }

        [Fact]
        public void Decrypt_WithWrongToken_ShouldThrowOrReturnGarbage()
        {
            // Arrange
            var rawNote = "Dado clínico sensível";
            var rightToken = "wctk_" + Guid.NewGuid().ToString("N");
            var wrongToken = "wctk_" + Guid.NewGuid().ToString("N");

            var encrypted = EncryptAes256(rawNote, rightToken);

            // Act
            var act = () => DecryptAes256(encrypted, wrongToken);

            // Assert — Com chave errada, deve lançar exceção de criptografia
            act.Should().Throw<Exception>("decifrar com token errado deve falhar");
        }

        [Fact]
        public void Encrypt_EmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N");

            // Act
            var encrypted = EncryptAes256("", rawToken);

            // Assert
            encrypted.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_EmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N");

            // Act
            var decrypted = DecryptAes256("", rawToken);

            // Assert
            decrypted.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Nota simples")]
        [InlineData("Nota com ç, ã, é, ü — caracteres UTF-8 especiais")]
        [InlineData("Diagnóstico: CID F84.0 — Autismo Infantil. Sessão ABA: 45min. Evolução positiva.")]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        public void EncryptDecrypt_VariousTexts_ShouldRoundTripCorrectly(string rawNote)
        {
            // Arrange
            var rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            // Act
            var encrypted = EncryptAes256(rawNote, rawToken);
            var decrypted = DecryptAes256(encrypted, rawToken);

            // Assert
            decrypted.Should().Be(rawNote);
        }

        // ----------------------------------------------------------------
        // Helpers — Replicam os métodos privados do AppService para teste white-box
        // ----------------------------------------------------------------

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string EncryptAes256(string plainText, string rawToken)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.KeySize = 256;
            using (var sha256 = SHA256.Create())
            {
                aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
            }
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new System.IO.MemoryStream();
            using (var cs = new System.Security.Cryptography.CryptoStream(ms, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
            using (var sw = new System.IO.StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            var iv = Convert.ToBase64String(aes.IV);
            var cipher = Convert.ToBase64String(ms.ToArray());
            return $"{iv}:{cipher}";
        }

        private static string DecryptAes256(string encryptedText, string rawToken)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

            var parts = encryptedText.Split(':');
            if (parts.Length != 2) return encryptedText;

            var iv = Convert.FromBase64String(parts[0]);
            var cipher = Convert.FromBase64String(parts[1]);

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.KeySize = 256;
            using (var sha256 = SHA256.Create())
            {
                aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
            }
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new System.IO.MemoryStream(cipher);
            using var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
            using var sr = new System.IO.StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
