using Newtonsoft.Json;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
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
        /// Call to the ICS Health Check endpoint. Returns a boolean
        /// to indicate whether the HTTP response was a 2xx code or not.
        /// </summary>
        public static bool ICSHealthCheck()
        {
            var icsHealthCheckResponse = ContactService.GetInstance().GET_HealthCheck().GetAwaiter().GetResult();
            AssertHttpResponseCode(icsHealthCheckResponse, isSuccessExpected: true, "ICS health check");
            return icsHealthCheckResponse.IsSuccessStatusCode;
        }

        /// <summary>
        /// Call to the ICS Anonymous Contact endpoint to retrieve an
        /// anonymous contact from Shield by a given contact ID.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload GetAnonymousFromShield(string contactId, bool isSuccessExpected)
        {
            var icsApiGetPersonResult = ContactService.GetInstance().GET_Anonymous(contactId).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiGetPersonResult, isSuccessExpected, "ICS Get Anonymous");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiGetPersonResult);
        }

        /// <summary>
        /// Call to the ICS Anonymous Contact endpoint to create a
        /// new contact record in Shield.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload CreateAnonymousInShield(ICSContactPayload requestBody, bool isSuccessExpected)
        {
            var icsApiCreatePersonResult = ContactService.GetInstance().POST_Anonymous(requestBody).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiCreatePersonResult, isSuccessExpected, "ICS Create Anonymous");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiCreatePersonResult);
        }

        /// <summary>
        /// Call to the ICS Anonymous Contact endpoint to update an
        /// existing contact record in Shield by a given contact ID.
        /// </summary>  
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload UpdateAnonymousInShield(ICSContactPayload requestBody, string contactId,  bool isSuccessExpected)
        {
            var icsApiUpdatePersonResult = ContactService.GetInstance().PUT_AnonymousById(requestBody, contactId).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiUpdatePersonResult, isSuccessExpected, "ICS Update Anonymous");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiUpdatePersonResult);
        }

        /// <summary>
        /// GET request that retrieves a contact based on the provided Person ID, via the ICS.
        /// ICS should retrieve the member personal info from Member Central, and the Insurance
        /// financial information from Shield.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload GetContactFromMemberCentral(string personId, bool isSuccessExpected)
        {
            var icsApiGetPersonResult = ContactService.GetInstance().GET_Person(personId).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiGetPersonResult, isSuccessExpected, "ICS Get Member from MC");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiGetPersonResult);
        }

        /// <summary>
        /// POST request that creates a contact in Member Central via ICS.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload CreateContactInMemberCentral(ICSContactPayload requestBody, bool isSuccessExpected)
        {
            var icsApiCreatePersonResult = ContactService.GetInstance().POST_Person(requestBody).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiCreatePersonResult, isSuccessExpected, "ICS Create Member in MC");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiCreatePersonResult);
        }

        /// <summary>
        /// Update the Person record in Member Central and updating banking information
        /// in Shield, via ICS.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload UpdateContactInMemberCentral(ICSContactPayload requestBody, bool isSuccessExpected)
        {
            var icsApiCreatePersonResult = ContactService.GetInstance().PUT_PersonByCRMId(requestBody, requestBody.PersonId).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiCreatePersonResult, isSuccessExpected, $"ICS Upate Member in MC of {requestBody.PersonId}");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiCreatePersonResult);
        }

        /// <summary>
        /// Call to the ICS endpoint to fetch a member record from
        /// Member Central by a given Person ID.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload GetPersonFromMemberCentral(string personId, bool isSuccessExpected)
        {
            var icsApiGetPersonResult = ContactService.GetInstance().GET_Person(personId).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiGetPersonResult, isSuccessExpected, "ICS Get Person");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiGetPersonResult);
        }

        /// <summary>
        /// Call to the ICS endpoint to create a new member record
        /// in Member Central with the provided data.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload CreatePersonInMemberCentral(ICSContactPayload requestBody, bool isSuccessExpected)
        {
            var icsApiCreatePersonResult = ContactService.GetInstance().POST_Person(requestBody).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiCreatePersonResult, isSuccessExpected, "ICS Create Person");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiCreatePersonResult);
        }

        /// <summary>
        /// Call to the ICS member match endpoint to find a member
        /// record based on the provided criteria in the requestBody.
        /// </summary>
        /// <param name="isSuccessExpected">If true, asserts the response to be a 2xx code. If false, asserts the response to be any failure code</param>
        public static ICSContactPayload MemberMatch(ICSMemberMatchPayload requestBody, bool isSuccessExpected)
        {
            var icsApiMemberMatchResult = ContactService.GetInstance().POST_MemberMatch(requestBody).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiMemberMatchResult, isSuccessExpected, "ICS Member Match");

            if (!isSuccessExpected) { return null; }

            return GetResponseContentByType<ICSContactPayload>(icsApiMemberMatchResult);
        }

        private async Task<HttpResponseMessage> GET_HealthCheck()
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

        private async Task<HttpResponseMessage> GET_Anonymous(string contactId)
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

        private async Task<HttpResponseMessage> POST_Anonymous(ICSContactPayload requestBody)
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

        private async Task<HttpResponseMessage> PUT_AnonymousById(ICSContactPayload requestBody, string contactId)
        {
            var apiResponse = new HttpResponseMessage();
            var bodyString = JsonConvert.SerializeObject(requestBody);

            try
            {
                apiResponse = await Put_Request(endpoint: Constant.Endpoint.AnonymousContactById(contactId), body: bodyString, addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

        private async Task<HttpResponseMessage> GET_Person(string personId)
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

        private async Task<HttpResponseMessage> POST_Person(ICSContactPayload requestBody)
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

        private async Task<HttpResponseMessage> PUT_PersonByCRMId(ICSContactPayload requestBody, string crmId)
        {
            var apiResponse = new HttpResponseMessage();
            var bodyString = JsonConvert.SerializeObject(requestBody);

            try
            {
                apiResponse = await Put_Request(endpoint: Constant.Endpoint.MCContactByCRMId(crmId), body: bodyString, addedHeaders: _apiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask(ex.Message);
            }
            return apiResponse;
        }

        private async Task<HttpResponseMessage> POST_MemberMatch(ICSMemberMatchPayload requestBody)
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

        private static void AssertHttpResponseCode(HttpResponseMessage httpResponse, bool isSuccessExpected, string apiContextString)
        {
            string responseCodeString = $"{(int)httpResponse.StatusCode} {httpResponse.StatusCode}";
            Reporting.IsTrue(httpResponse.IsSuccessStatusCode == isSuccessExpected, $"{apiContextString} successful. Received '{responseCodeString}' from request");
        }

        private static T GetResponseContentByType<T>(HttpResponseMessage httpResponse)
        {
            return JsonConvert.DeserializeObject<T>(httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }
    }
}
