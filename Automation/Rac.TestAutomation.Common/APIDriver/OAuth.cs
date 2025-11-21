using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public class OAuth : BaseAPI
    {
        public  static OAuth  Instance { get; private set; }
        private static string _oAuthToken;

        private string _tenantId;
        private string _clientId;
        private string _clientSecret;
        private string _scope;

        private const string MS_AUTH_URL = "https://login.microsoftonline.com/";

        /// <summary>
        /// Fetches an instance of the OAuth service,
        /// </summary>
        /// <param name="tenantId">This is the tenant ID for RAC WA services with MS Azure. Should be in the config.</param>
        /// <param name="targetedAzureService">The Azure service for which we want to get an OAuth token for.</param>
        /// <returns></returns>
        public static OAuth GetInstance(string tenantId, AzureEnvironment targetedAzureService)
        {
            Instance = Instance ?? new OAuth(tenantId, targetedAzureService);

            return Instance;
        }

        public OAuth(string tenantId, AzureEnvironment targetedAzureService) : base()
        {
            if (string.IsNullOrEmpty(tenantId))
            { Reporting.Error("Tenant ID needs to be defined in the config.json for services that require an OAuth token."); }
            if (string.IsNullOrEmpty(targetedAzureService?.APIClientId))
            { Reporting.Error("The config for the targeted Azure service has not provided a Client ID, so we cannot generate a OAuth token."); }
            if (string.IsNullOrEmpty(targetedAzureService?.APISecret))
            { Reporting.Error("The config for the targeted Azure service has not provided a API Secret key, so we cannot generate a OAuth token."); }
            if (string.IsNullOrEmpty(targetedAzureService?.APIScope))
            { Reporting.Error("A scope was not provided by the config for the targeted Azure service, so we cannot generate a OAuth token."); }

            _tenantId     = tenantId;
            _clientId     = targetedAzureService?.APIClientId;
            _clientSecret = targetedAzureService?.APISecret;
            _scope        = targetedAzureService?.APIScope;
        }

        /// <summary>
        /// Gets an OAuth token from Microsoft's Identity Service.
        /// </summary>
        /// <returns>Token if successful, otherwise will return an empty string</returns>
        public async Task<string> GetOAuthToken()
        {
            // If we've recently generated this for past requests, then
            // no need to generate a new one.
            if (!string.IsNullOrEmpty(_oAuthToken)) { return _oAuthToken; }

            string[] scopes = new string[] { _scope };

            AuthenticationResult authResult = null;

            var app = ConfidentialClientApplicationBuilder.Create(_clientId)
                                                      .WithClientSecret(_clientSecret)
                                                      .WithAuthority(new Uri($"{MS_AUTH_URL}{_tenantId}"))
                                                      .Build();

            try
            {
                authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Reporting.LogAsyncTask($"Error acquiring token silently:{Reporting.HTML_NEWLINE}{ex}");
                return string.Empty;
            }

            if (authResult != null)
            {
                Reporting.LogAsyncTask($"Access token:{Reporting.HTML_NEWLINE}{authResult.AccessToken}");
                _oAuthToken = authResult.AccessToken;
                return _oAuthToken;
            }
            return string.Empty;
        }
    }
}
