using System.ComponentModel.DataAnnotations;

namespace WeCare.CrossTenantAccess
{
    public class VerifyConsentTokenDto
    {
        [Required]
        public string RawToken { get; set; }
    }
}
