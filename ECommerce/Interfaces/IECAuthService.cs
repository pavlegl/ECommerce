using System.Security.Claims;

namespace ECommerce
{
    public interface IECAuthService
    {
        /// <summary>
        /// Contains information about the SecretKey, algorithm and other 
        /// </summary>
        IECAuthContainerModel AuthContainerModel { get; set; }
        
        bool IsTokenValid(string token);
        string GenerateToken();
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
