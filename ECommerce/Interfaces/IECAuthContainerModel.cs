using System.Security.Claims;

namespace ECommerce
{
    public interface IECAuthContainerModel
    {
        string SecretKeyBase64 { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }

        Claim[] Claims { get; set; }
    }
}
