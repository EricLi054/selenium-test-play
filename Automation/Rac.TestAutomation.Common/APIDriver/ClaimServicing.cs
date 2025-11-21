using Newtonsoft.Json;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common.APIDriver
{
    public class ClaimServicing : BaseAPI
    {
        private static class Header
        {            
            public const string CRMId = "RACWA-CRM-ID";
            public const string SessionId = "SessionId";
            public const string Shield = "ShieldEnvironment";
            public const string MCMock = "Feature_UseMCMock";
            public const string UserAgent = "User-Agent";
        }

        private static class Parameter
        {
            public const string ClaimNumber = "claimNumber";           
        }

        private static class Paths
        {
            public const string CreateSession = "insurance/claimsservicing/api/v1/session";
            public const string ValidateUploadInvoice = "insurance/claimsservicing/api/v1/upload-invoice/validate";
        }

        private Dictionary<string, string> _apiHeaders;

        public ClaimServicing(String personId) : base()
        {
            var config = Config.Get();
            var zone = "sit";
            if (config.Shield.IsUatEnvironment())
            { zone = "uat"; }
            else if (config.Shield.IsDevEnvironment())
            { zone = "dev"; }
            _baseUrl = string.Format($"https://az-api-{zone}.ractest.com.au/");

            _apiHeaders = new Dictionary<string, string>()
            {                 
                 { Header.CRMId, personId },
                 { Header.UserAgent, $"az-api-{zone}.ractest.com.au" }
            };
        }

        public async Task<string> POST_CreateSession()
        {
            var apiCall = await Post_Request(endpoint: Paths.CreateSession, addedHeaders: _apiHeaders);
            string session = null;

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                session = await apiCall.Content.ReadAsStringAsync();
                session = session.Replace("\"", "");
            }
            else
            {
                Reporting.Error($"Failed to create an session, error: {apiCall.StatusCode}");
            }

            return session;
        }

        public async Task<ValidateUploadInvoice> GET_ValidateUploadInvoice(string claimNumber, string sessionid)
        {
            ValidateUploadInvoice response = null;
            
            _apiHeaders.Add(Header.SessionId, sessionid);
            _apiHeaders.Add(Header.Shield, Config.Get().Shield.Environment.ToUpper());
            _apiHeaders.Add(Header.MCMock, Config.Get().IsMCMockEnabled().ToString());           

            var parameters = new Dictionary<string, string>();
            DataHelper.AddParamIfNotEmpty(parameters, Parameter.ClaimNumber, claimNumber);

            var apiCall = await Get_Request(endpoint: Paths.ValidateUploadInvoice, addedHeaders: _apiHeaders, addedParams: parameters);

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                var raw = await apiCall.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<ValidateUploadInvoice>(raw);
            }
            else
            {
                Reporting.Error($"Failed to create an session, error: {apiCall.StatusCode}");
            }

            return response;
        }


        public class ValidateUploadInvoice
        {
            [JsonProperty("isClaimExpired")]
            public bool IsClaimExpired { get; set; }

            [JsonProperty("contactFirstName")]
            public string ContactFirstName { get; set; }

            [JsonProperty("token")]
            public string Token { get; set; }
        }


    }
}
