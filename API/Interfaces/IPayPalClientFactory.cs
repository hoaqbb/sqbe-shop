using PaypalServerSdk.Standard;

namespace API.Interfaces
{
    public interface IPayPalClientFactory
    {
        PaypalServerSdkClient CreateClient();
    }
}
