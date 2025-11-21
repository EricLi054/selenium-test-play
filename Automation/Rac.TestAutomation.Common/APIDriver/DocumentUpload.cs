using AventStack.ExtentReports.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.APIDriver.ClaimServicing;

namespace Rac.TestAutomation.Common.APIDriver
{
    public class DocumentUpload : BaseAPI
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
            public const string Token = "token";
        }

        private static class Paths
        {
            public const string CreateSession = "insurance/document-service/api/v1/session";
            public const string UploadDocumentConfig = "insurance/document-service/api/v1/upload/configuration";            
        }

        private Dictionary<string, string> _apiHeaders;       

        public DocumentUpload(String personId) : base()
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
            }
            else
            {
                Reporting.Error($"Failed to create an session, error: {apiCall.StatusCode}");
            }

            return session;
        }

        public async Task<int> GET_UploadInvoiceConfig(string token, string sessionid)
        {
            RemainingFile response = null;
           
            _apiHeaders.Add(Header.SessionId, sessionid);
            _apiHeaders.Add(Header.Shield, Config.Get().Shield.Environment);
            _apiHeaders.Add(Header.MCMock, Config.Get().IsMCMockEnabled().ToString());            

            var parameters = new Dictionary<string, string>();
            DataHelper.AddParamIfNotEmpty(parameters, Parameter.Token, token);

            var apiCall = await Get_Request(endpoint: Paths.UploadDocumentConfig, addedHeaders: _apiHeaders, addedParams: parameters);

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                var raw = await apiCall.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<RemainingFile>(raw);
            }

            return response.RemainingFiles;
        }

        public class RemainingFile
        {
            [JsonProperty("remainingFiles")]
            public int RemainingFiles { get; set; }
        }

    }
}
