using Newtonsoft.Json;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common.APIDriver
{
    
    public class ContactService : BaseAPI
    {
        private class Constant
        {
            public class Endpoint
            {
                public const string AnonymousContact = "insurance/contact-service/api/v1/anonymous";
                public const string MemberMatch      = "insurance/contact-service/api/v1/contacts/match";
                public const string MCContact        = "insurance/contact-service/api/v1/contacts";
                public const string IsAlive          = "insurance/contact-service/api/v1/health/status";
                public static string AnonymousContactById(string id) => $"{AnonymousContact}/{id}";
                public static string ConvertContact(string id)       => $"{AnonymousContact}/{id}/conversion";
                public static string MCContactByCRMId(string CRMid)  => $"{MCContact}/{CRMid}";
            }
            public class Header
            {
                public const string Shield    = "Environment";
                public const string ApiKey    = "Ocp-Apim-Subscription-Key";
                public const string Source    = "SourceSystem";
                public const string Host      = "Host";
                public const string UseMCMock = "Feature_UseMCMock";
                public const string PersonV3  = "Feature_PersonV3";
            }
        }

        public static ContactService Instance { get; private set; }

        private Dictionary<string, string> _apiHeaders;
        private static readonly object _lock = new object();

        public ContactService() : base()
        {
            var config = Config.Get();
            _baseUrl = string.Format($"https://az-api-{config.Azure.ContactService.APIEnv}.ractest.com.au/");

            _apiHeaders = new Dictionary<string, string>()
            {
                { Constant.Header.Source, "IntegrationTests" },
                { Constant.Header.ApiKey, config.Azure.ContactService.APIKey },
                { Constant.Header.Shield, config.Shield.Environment },
                { Constant.Header.Host,   $"az-api-{config.Azure.ContactService.APIEnv}.ractest.com.au" },
                { Constant.Header.UseMCMock, config.IsMCMockEnabled().ToString() },
                { Constant.Header.PersonV3,  config.IsPersonV3Enabled().ToString() }
            };
        }

        public static ContactService GetInstance()
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    Instance = new ContactService();                   
                }
            }

            return Instance;
        }

        /// <summary>
        /// Health check or Is Alive check.
        /// </summary>
        public async Task<HttpResponseMessage> GET_HealthCheck()
        {
            var apiResponse = new HttpResponseMessage();
            try
            {
                apiResponse = await Get_Request(endpoint: Constant.Endpoint.IsAlive, addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }

            return apiResponse;
        }

        /// <summary>
        /// Retrieve an anonymous contact from Shield by Contact ID.
        /// </summary>
        public async Task<HttpResponseMessage> GET_Anonymous(string contactId)
        {
            var apiResponse = new HttpResponseMessage();

            try
            {
                apiResponse = await Get_Request(endpoint: Constant.Endpoint.AnonymousContactById(contactId), addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

        /// <summary>
        /// Create a new anonymous contact in Shield.
        /// </summary>
        public async Task<HttpResponseMessage> POST_Anonymous(ICSContactPayload requestBody)
        {
            var apiResponse = new HttpResponseMessage();
            var bodyString = JsonConvert.SerializeObject(requestBody);

            try
            {
                apiResponse = await Post_Request(endpoint: Constant.Endpoint.AnonymousContact, body: bodyString, addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

        /// <summary>
        /// Retrieve a existing member from Member Central by Person ID.
        /// </summary>
        public async Task<HttpResponseMessage> GET_Person(string personId)
        {
            var apiResponse = new HttpResponseMessage();

            try
            {
                apiResponse = await Get_Request(endpoint: Constant.Endpoint.MCContactByCRMId(personId), addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

        /// <summary>
        /// Create a new person record in Member Central.
        /// </summary>
        public async Task<HttpResponseMessage> POST_Person(ICSContactPayload requestBody)
        {
            var apiResponse = new HttpResponseMessage();
            var bodyString = JsonConvert.SerializeObject(requestBody);

            try
            {
                apiResponse = await Post_Request(endpoint: Constant.Endpoint.MCContact, body: bodyString, addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

        public async Task<HttpResponseMessage> POST_MemberMatch(ICSMemberMatchPayload requestBody)
        {
            var apiResponse = new HttpResponseMessage();
            var bodyString = JsonConvert.SerializeObject(requestBody);

            try
            {
                apiResponse = await Post_Request(endpoint: Constant.Endpoint.MemberMatch, body: bodyString, addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

    }
}
