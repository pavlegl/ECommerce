using System.Security.Claims;

namespace ECommerce
{
    public abstract class ECAuthService
    {
        public ECAuthService(IECAuthContainerModel model)
        {
        }

        abstract public bool IsTokenValid(string token);
        abstract public string GenerateToken();
        abstract public IEnumerable<Claim> GetTokenClaims(string token);
    }
}
