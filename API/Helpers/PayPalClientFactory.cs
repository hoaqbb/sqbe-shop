using API.Interfaces;
using Microsoft.Extensions.Options;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;

namespace API.Helpers
{
    public class PayPalClientFactory : IPayPalClientFactory
    {
        private readonly IOptions<PayPalSettings> _config;

        public PayPalClientFactory(IOptions<PayPalSettings> config)
        {
            _config = config;
        }

        public PaypalServerSdkClient CreateClient()
        {
            var environment = _config.Value.Mode == "Sandbox"
                ? PaypalServerSdk.Standard.Environment.Sandbox
                : PaypalServerSdk.Standard.Environment.Production;

            PaypalServerSdkClient client = new PaypalServerSdkClient.Builder()
                .ClientCredentialsAuth(
                    new ClientCredentialsAuthModel.Builder(
                        _config.Value.ClientId,
                        _config.Value.ClientSecret
                    )
                    .Build()).Environment(environment)
                .Build();

            return client;
        }
    }
}
