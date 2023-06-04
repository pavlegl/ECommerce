using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ECommerce.IdentityAPI.BL
{
    public class JwtContainerModel : IECAuthContainerModel
    {
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 1440;

        public Claim[] Claims { get; set; }
    }
}
