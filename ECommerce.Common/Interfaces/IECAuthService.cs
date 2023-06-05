using System.Security.Claims;

namespace ECommerce
{
    public interface IECAuthService
    {
        IECAuthContainerModel AuthContainerModel { get; set; }
        
        bool IsTokenValid(string token);
        string GenerateToken();
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
