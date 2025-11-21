using Newtonsoft.Json;
using NUnit.Framework.Internal;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common
{
    public class MemberCentral : BaseAPI
    {
        public static MemberCentral Instance { get; private set; }

        private class Constants
        {
            public class API
            {
                public class Header
                {
                    public const string CorrelationID = "correlationId";
                    public const string ApiKey = "Ocp-Apim-Subscription-Key";
                    public const string Source = "sourceSystem";
                    public const string Shield = "ShieldEnvironment";
                    public const string FeatureEnvName = "FeatureEnvName";
                }

                public class PersonV2
                {
                    private const string _PersonBase = "person/v2/";
                    public const string Match        = _PersonBase + "match";
                    public const string ByExternalID = _PersonBase + "personbyexternalid";
                    public const string Person       = _PersonBase + "person/";
                    public const string Products     = _PersonBase + "products";
                }
                public class McMock
                {
                    private const string _MockBase = "mcmockutility/api/v1/";
                    public const string SetMatchResult = _MockBase + "match";
                    public const string SetRoadside    = _MockBase + "roadside";
                    public const string IsAlive        = _MockBase + "is-alive";
                }
                public class Parameters
                {
                    public const string FirstName     = "FirstName";
                    public const string MiddleName    = "MiddleName";
                    public const string DateOfBirth   = "DateOfBirth";
                    public const string Surname       = "Surname";
                    public const string MobilePhone   = "MobilePhone";
                    public const string PersonalEmail = "PersonalEmail";
                    public const string StreetName    = "StreetName";
                    public const string Suburb        = "Suburb";
                    public const string Postcode      = "Postcode";
                    public const string System        = "System";
                    public const string ExtSystemId   = "ExternalSystemId";
                    public const string PersonIdList  = "PersonIdList";
                }
            }
        }

        private static readonly object _lock = new object();

        private Dictionary<string, string> _mcApiHeaders;

        private MemberCentral() : base()
        {
            var config = Config.Get();

            _baseUrl = string.Format($"https://az-api-{config.Azure.MemberCentral.APIEnv}.ractest.com.au/");

            _mcApiHeaders = new Dictionary<string, string>()
            {
                {Constants.API.Header.CorrelationID, $"mcmock{DateTime.Now.ToString("s")}"},
                {Constants.API.Header.Source, "SHIELD"},
                {Constants.API.Header.ApiKey, config.Azure.MemberCentral.APIKey},
                {Constants.API.Header.Shield, config.Shield.Environment}
            };

            if (config.IsMCMockEnabled())
            { 
                _mcApiHeaders.Add(Constants.API.Header.FeatureEnvName, "InsuranceMock");
                Reporting.LogAsyncTask($"FeatureEnvName = InsuranceMock");
            }

            return;
        }

        public static MemberCentral GetInstance()
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    if (Instance == null)
                    {
                        Instance = new MemberCentral();
                    }
                }
            }

            return Instance;
        }

        /// <summary>
        /// Checks contact details against Member Central Person v2 Match API.
        /// It will use all the known information that we can expect of a member
        /// when at the point of needing to do a match. Member Central will use
        /// its ruleset in find a match.
        /// More information at: https://rac-wa.atlassian.net/wiki/spaces/BEN/pages/956663250/Matching+API+Rules
        /// If contact does not return as a single match for the given
        /// contact ID, then we return a null.
        /// </summary>
        /// <param name="contact">Contact object to find data to match on. All fields are treated as optional, though Person API may have its own requirements.</param>
        public async Task<API_MemberCentralPersonV2Response> GET_MemberByMemberMatch(Contact contact)
        {
            API_MemberCentralPersonV2Response memberData = null;
            var apiResponse = PerformMultiMatchCall(contact).Result;

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                var raw = await apiResponse.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Response>(raw);

                //only return the response if the Shield contact id exist in the member match response and contact have a valid gender, otherwise return null
                if (jsonResponse.IsExpectedShieldIdLinkedWithThisCRMRecord(contact.Id) && ((jsonResponse.Gender == Gender.Female.ToString()) || (jsonResponse.Gender == Gender.Male.ToString())))
                {
                    memberData = jsonResponse;
                }
            }

            return memberData;
        }

        /// <summary>
        ///Get member details by PersonId/ CRM Id
        /// </summary>
        /// <param name="personId"></param>       
        /// <returns></returns>
        /// <exception cref="NUnitException">thrown if a non-success code received from Person API</exception>
        public async Task<API_MemberCentralPersonV2Response> GET_PersonByPersonId(String personId)
        {
            API_MemberCentralPersonV2Response response = null;
            var endpoint = Constants.API.PersonV2.Person + personId;
            
            var apiCall = await Get_Request(endpoint: endpoint, addedHeaders: _mcApiHeaders);

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                var raw = await apiCall.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Response>(raw);
            }
            else
            {
                throw new NUnitException($"Failed in communication with MemberCentral API, error: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }

            return response;
        }

        /// <summary>
        /// Update contact's mobile phone record in MC
        /// </summary>
        /// <param name="currentMCRecord">We expect caller has already fetched current record from MC, and we will use this if talking to Mock instead of real MC</param>
        /// <param name="newMobileNumber">New mobile number to update in Member Central/Mock</param>
        /// <returns>Payload of the response from Person API</returns>
        /// <exception cref="NUnitException">thrown if a non-success code received from Person API</exception>
        public async Task<API_MemberCentralPersonV2Response> PUT_UpdateMemberMobile(API_MemberCentralPersonV2Response currentMCRecord, String newMobileNumber)
        {
            var endpoint = Constants.API.PersonV2.Person + currentMCRecord.PersonId;
            var jsonBody = string.Empty;

            if (Config.Get().IsMCMockEnabled())
            {
                // Member Central Mock requires most of the payload to do
                // a successful member record update.
                currentMCRecord.MobilePhone = newMobileNumber;
                jsonBody = JsonConvert.SerializeObject(currentMCRecord);
            }
            else
            {
                // Real Member Central can handle just the changed fields in the payload.
                jsonBody = JsonConvert.SerializeObject(new { MobilePhone = newMobileNumber });
            }

            var apiCall = await Put_Request(endpoint: endpoint, body: jsonBody, addedHeaders: _mcApiHeaders);

            if (apiCall.StatusCode != HttpStatusCode.OK)
            {
                throw new NUnitException($"Failed in communication with MemberCentral API, error: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                Reporting.LogAsyncTask($"Successful update via MemberCentral API, response code: {apiCall.StatusCode}");
            }
            var raw = await apiCall.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Response>(raw);
            return response;
        }

        /// <summary>
        /// Update contact email addess in MC
        /// </summary>
        /// <param name="currentMCRecord">We expect caller has already fetched current record from MC, and we will use this if talking to Mock instead of real MC</param>
        /// <param name="newEmailAddress">New email address to update in Member Central/Mock</param>
        /// <returns>Payload of the response from Person API</returns>
        /// <exception cref="NUnitException">thrown if a non-success code received from Person API</exception>
        public async Task<API_MemberCentralPersonV2Response> PUT_UpdateMemberEmailAddress(API_MemberCentralPersonV2Response currentMCRecord, String newEmailAddress)
        {
            var endpoint = Constants.API.PersonV2.Person + currentMCRecord.PersonId;
            var jsonBody = string.Empty;

            if (Config.Get().IsMCMockEnabled())
            {
                // Member Central Mock requires most of the payload to do
                // a successful member record update.
                currentMCRecord.PersonalEmailAddress = newEmailAddress;
                jsonBody = JsonConvert.SerializeObject(currentMCRecord);
            }
            else
            {
                // Real Member Central can handle just the changed fields in the payload.
                jsonBody = JsonConvert.SerializeObject(new { PersonalEmailAddress = newEmailAddress });
            }

            var apiCall = await Put_Request(endpoint: endpoint, body: jsonBody, addedHeaders: _mcApiHeaders);

            if (apiCall.StatusCode != HttpStatusCode.OK)
            {
                throw new NUnitException($"Failed in communication with MemberCentral API, error: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }
            var raw = await apiCall.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Response>(raw);
            return response;
        }

        /// <summary>
        /// Update the value of the Title on a member record in Member Central.
        /// </summary>
        /// <param name="currentMCRecord">We expect caller has already fetched current record from MC, and we will use this if talking to Mock instead of real MC</param>
        /// <param name="newTitle">The new title to be set.</param>
        /// <returns>Payload of the response from Person API</returns>
        /// <exception cref="NUnitException">Thrown if a non-success code is received from the Person API.</exception>
        public async Task<API_MemberCentralPersonV2Response> PUT_UpdateContactTitle(API_MemberCentralPersonV2Response currentMCRecord, String newTitle)
        {
            var endpoint = Constants.API.PersonV2.Person + currentMCRecord.PersonId;
            var jsonBody = string.Empty;

            if (Config.Get().IsMCMockEnabled())
            {
                // Member Central Mock requires most of the payload to do
                // a successful member record update.
                currentMCRecord.Title = newTitle;
                jsonBody = JsonConvert.SerializeObject(currentMCRecord);
            }
            else
            {
                // Real Member Central can handle just the changed fields in the payload.
                jsonBody = JsonConvert.SerializeObject(new { Title = newTitle });
            }

            var apiCall = await Put_Request(endpoint: endpoint, body: jsonBody, addedHeaders: _mcApiHeaders);

            if (apiCall.StatusCode != HttpStatusCode.OK)
            {
                throw new NUnitException($"Failed to update title in Member Central: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }
            var raw = await apiCall.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Response>(raw);
            return response;
        }

        /// <summary>
        ///Get member details by PersonId/ CRM Id
        /// </summary>
        /// <param name="personId"></param>       
        /// <exception cref="NUnitException">thrown if a non-success code received from Person API</exception>
        public async Task<API_MemberCentralPersonV2Products> GET_ProductHoldingsByPersonId(String personId)
        {
            API_MemberCentralPersonV2Products response = null;
            var endpoint = Constants.API.PersonV2.Products + personId;

            var parameters = new Dictionary<string, string>();
            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.PersonIdList, personId);

            var apiCall = await Get_Request(endpoint: Constants.API.PersonV2.Products, addedHeaders: _mcApiHeaders, addedParams: parameters);

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                var raw = await apiCall.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Products>(raw);
            }
            else
            {
                throw new NUnitException($"Failed in communication with MemberCentral API, error: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }

            return response;
        }

        /// <summary>
        /// Checks contact details against Member Central Person v2 Match API,
        /// to confirm the contact is a Multi-Match (if api call returns the StatusCode 'Conflict')
        /// More information at: https://rac-wa.atlassian.net/wiki/spaces/BEN/pages/956663250/Matching+API+Rules
        /// </summary>
        /// <param name="contact">Contact object to find data to match on. All fields are treated as optional, though Person API may have its own requirements.</param>
        public bool IsMultiMatch(Contact contact)
        {
            var apiResponse = PerformMultiMatchCall(contact).Result;

            /* Users of this method are typically verifying a contact
             * record that is expected to already be present. In essence
             * generally we expect a record to be MultiMatch and we're
             * just checking that is actually the case before the test
             * tries to use it. If we don't get a 409 or 200 response
             * then the data is in an unexpected state, so log it to
             * help debugging. */
            if (apiResponse.StatusCode != HttpStatusCode.Conflict &&
                apiResponse.StatusCode != HttpStatusCode.OK)
            { Reporting.Log($"For contact CRM ID:{contact.PersonId} we got a {apiResponse.StatusCode} response."); }
            return apiResponse.StatusCode == HttpStatusCode.Conflict;
        }

        /// <summary>
        /// Check if MC Mock feature toggle is defined and enabled, if not do nothing.
        /// Ensure the MC Mock table storage is up to date for the Contact used
        /// by sending a GET Person request through for that Shield Contact Id.
        /// </summary>
        /// <param name="ContactId">The Shield Contact Id identified for use in this test.</param>
        /// <returns>The Contact that was populated, or NULL if Mock is not enabled</returns>
        public static Contact PopulateMockMemberCentralWithLatestContactIdInfo(string ContactId)
        {
            if (!Config.Get().IsMCMockEnabled())
            { 
                return null;
            }
            var populatedContact = Contact.InitFromMCByShieldId(ContactId);
            Reporting.Log($"Populate Mock Member Central with Latest ContactId Info invoked successfully for ContactId {ContactId}. PersonId = {populatedContact.PersonId}");
            return populatedContact;
        }

        /// <summary>
        /// Check if MC Mock feature toggle is defined and enabled, if yes then  
        /// by sending a GET Person request will create the person in mc mock and
        /// if it's not enabled then update the same email address for the member
        /// this will trigger the contact sync between member central and shield
        /// Normalise the Title after get Person
        /// </summary>       
        public static bool SyncMemberCentralWithShield(string ContactId)
        {
            var getPersonResponse = Task.Run(() => GetInstance().GET_PersonByShieldContactId(ContactId.ToString())).GetAwaiter().GetResult();
            bool mcResult = false;
            if (getPersonResponse != null)
            {
                if (string.IsNullOrEmpty(getPersonResponse.Title))
                {
                    Reporting.Log($"Record does not appear to have an existing Title, will attempt " +
                        $"to continue without substituting value B2C Valid Title");
                }
                else if (!DataHelper.IsValidTitle(getPersonResponse.Title))
                {
                    Reporting.Log($"Existing Title '{getPersonResponse.Title}' is incompatible with B2C/Spark, " +
                        $"so we are updating member record {getPersonResponse.PersonId} to 'Mx' in case it comes up during the test.");
                    Task.Run(() => GetInstance().PUT_UpdateContactTitle(getPersonResponse, Title.Mx.GetDescription())).GetAwaiter().GetResult();
                }
                else
                {
                    Reporting.Log($"Existing Title '{getPersonResponse.Title}' is valid for B2C/Spark usage, will use " +
                        $"as-is for this test.");
                }

                if (!Config.Get().IsMCMockEnabled())
                {
                    Task.Run(() => GetInstance().PUT_UpdateMemberEmailAddress(getPersonResponse, getPersonResponse.PersonalEmailAddress)).GetAwaiter().GetResult();
                    mcResult = true;
                    Reporting.Log($"Sync Shield contact with Member Central successfully for ContactId {ContactId}. PersonId = {getPersonResponse.PersonId}");
                }
                else
                {
                    mcResult = true;
                    Reporting.Log($"Populate Mock Member Central with Latest ContactId Info invoked successfully for ContactId {ContactId}. PersonId = {getPersonResponse.PersonId}");
                }
            }
            return mcResult;
        }

        /// <summary>
        /// Send an APIM - GetPersonByExternalId request through so when we are using
        /// the MC Mock service we will ensure that the Contact being used is populated
        /// in the Table Storage for the Mock.
        /// 
        /// This should also allow for general use of the deserialized JSON in the respone returned
        /// by the Mock Member Central instance.
        /// </summary>
        /// <param name="PreferredShieldContactId">The Contact ID obtained for this test.</param>
        /// <returns=>Contact information matching the Contact Id provided</returns>
        public async Task<API_MemberCentralPersonV2Response>GET_PersonByShieldContactId(string PreferredShieldContactId)
        {
            API_MemberCentralPersonV2Response response = null;

            var parameters = new Dictionary<string, string>() {
                { Constants.API.Parameters.System, "shield" },
                { Constants.API.Parameters.ExtSystemId, PreferredShieldContactId }
            };
            try
            {
                var apiCall = await Get_Request(endpoint: Constants.API.PersonV2.ByExternalID, addedHeaders: _mcApiHeaders, addedParams: parameters);
                
                /* We normally only accept a 200 response from real MC
                 * For MC Mock, because the user and automation can force various states for testing
                 * then we'll accept any response code except the major errors; 400/401/404/500. */
                if (apiCall.StatusCode == HttpStatusCode.OK ||
                    (Config.Get().IsMCMockEnabled() && (apiCall.StatusCode != HttpStatusCode.BadRequest ||
                                                        apiCall.StatusCode != HttpStatusCode.InternalServerError ||
                                                        apiCall.StatusCode != HttpStatusCode.NotFound ||
                                                        apiCall.StatusCode != HttpStatusCode.Unauthorized)))
                {
                    response = JsonConvert.DeserializeObject<API_MemberCentralPersonV2Response>(apiCall.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is HttpRequestException)
            {
                Reporting.LogAsyncTask($"Failed in communication with MemberCentral API for GET Person V2 By Shield Contact Id: {PreferredShieldContactId}, the exception message is {ex}");
            }

            return response;
        }

        
        /// <summary>
        /// Check if MC Mock feature toggle is defined and enabled, if not do nothing.
        /// Update the response code to be returned for the Contact provided 
        /// by sending a PUT Utility - Set Match request through for that Shield Contact Id.
        /// </summary>
        /// <param name="ContactId">The Shield Contact Id identified for use in this test.</param>
        /// <param name="httpResponseStatusCode">The HTTP Response code required by the test (e.g. "409" for Multimatch)</param>
        public static void SetMultiMatchResponseByShieldContactId(string ContactId, int httpResponseStatusCode)
        {
            var config = Config.Get();
            if (!config.IsMCMockEnabled())
            {
                return;
            }
            var requestContact = ContactId;
            Task.Run(() => GetInstance().SetMockResponseCodeByShieldContactId(requestContact, httpResponseStatusCode.ToString())).GetAwaiter().GetResult();
            Reporting.Log($"Member Central Mock should return Status Code 409: Multi Match for Shield Contact Id: {ContactId} against {config.Shield.Environment.ToUpper()}.");
        }

        /// <summary>
        /// Modify RSA (Road-Side Assistance) membership level recorded for a contact
        /// in Member Central.
        /// </summary>
        /// <param name="contactId">Contact ID to modify in MC Mock.</param>
        /// <param name="roadside">Use constants from Constants.MemberCentral.Roadside</param>
        public static void SetMockContactRoadsideAssistanceLevel(string contactId, string roadside)
        {
            var config = Config.Get();
            if (!config.IsMCMockEnabled())
            {
                return;
            }
            var requestContact = contactId;
            Task.Run(() => GetInstance().SetMockRoadsideAssistanceByShieldContactId(requestContact, roadside)).GetAwaiter().GetResult();
            Reporting.Log($"Member Central Mock should return Roadside Assistance level {roadside} for Shield Contact Id: {contactId} against {config.Shield.Environment.ToUpper()}.");
        }

        private async Task<Contact> SetMockResponseCodeByShieldContactId(string PreferredShieldContactId, string httpResponseStatusCode)
        {
            var config = Config.Get();
            var mcMockHeaders = GenerateHeadersForMCMock();
            Contact multiMatchContact = null;
            var bodyContent = new Dictionary<string, string>()
            {
                { "ContactId", PreferredShieldContactId },
                { "StatusCode", httpResponseStatusCode }
            };
            string jsonBody = JsonConvert.SerializeObject(bodyContent);

            var apiCall = await Put_Request(endpoint: Constants.API.McMock.SetMatchResult, addedHeaders: mcMockHeaders, body: jsonBody);
            if (apiCall.StatusCode == HttpStatusCode.NoContent)
            {
                Reporting.LogAsyncTask($"HTTP response from SET MATCH RESPONSE API = {apiCall.StatusCode}");
            }
            else
            {
                throw new NUnitException($"Failed in communication with MC Mock API, error: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }
            return multiMatchContact;
        }

        private async Task SetMockRoadsideAssistanceByShieldContactId(string preferredShieldContactId, string roadside)
        {
            var mcMockHeaders = GenerateHeadersForMCMock();
            var bodyContent = new Dictionary<string, string>()
            {
                { "contactId", preferredShieldContactId },
                { "roadsideAssistanceId", roadside }
            };
            string jsonBody = JsonConvert.SerializeObject(bodyContent);

            var apiCall = await Put_Request(endpoint: Constants.API.McMock.SetRoadside, addedHeaders: mcMockHeaders, body: jsonBody);
            if (apiCall.StatusCode == HttpStatusCode.NoContent)
            {
                Reporting.LogAsyncTask($"HTTP response from SET RSA LEVEL API = {apiCall.StatusCode}");
            }
            else if (apiCall.StatusCode == HttpStatusCode.NotFound)
            {
                Reporting.LogAsyncTask($"HTTP response from SET RSA LEVEL API for {preferredShieldContactId} was not found, check that it has been loaded into Mock first.");
            }
            else
            {
                throw new NUnitException($"Failed in communication with MC Mock API for SET RSA LEVEL, error: {apiCall.StatusCode} {apiCall.Content.ReadAsStringAsync().Result}");
            }
            return;
        }

        /// <summary>
        /// Call the Member Central Mock service to see if it is currently alive.
        /// </summary>
        /// <returns>TRUE if we get a 204 response, otherwise FALSE.</returns>
        public static bool IsMCMockAlive()
        {
            var config = Config.Get();
            var api = new MemberCentral();
            return api.MockIsAlive().Result;
        }

        private async Task<bool> MockIsAlive()
        {
            var mcMockHeaders = GenerateHeadersForMCMock();

            var apiCall = await Get_Request(endpoint: Constants.API.McMock.IsAlive, addedHeaders: mcMockHeaders);
            Reporting.LogAsyncTask($"HTTP MCMock IsAlive = {apiCall.StatusCode}");
            return apiCall.StatusCode == HttpStatusCode.NoContent;
        }

        private Dictionary<string, string> GenerateHeadersForMCMock()
        {
            var config = Config.Get();
            var myToken = OAuth.GetInstance(config.Azure.TenantId, config.Azure.MCMock).GetOAuthToken().Result;

            if (string.IsNullOrEmpty(myToken))
            { Reporting.Error("We failed to get an OAuth token, unable to proceed."); }

            var newHeaders = new Dictionary<string, string>();
            foreach(var item in _mcApiHeaders)
            {
                if (item.Key == Constants.API.Header.ApiKey)
                {
                    newHeaders.Add(Constants.API.Header.ApiKey, config.Azure.MCMock.APIKey);
                }
                else
                { newHeaders.Add(item.Key, item.Value); }
            }

            newHeaders.Add("Authorization", $"Bearer {myToken}");
            return newHeaders;
        }

        private Task<HttpResponseMessage> PerformMultiMatchCall(Contact contact)
        {
            var parameters = new Dictionary<string, string>();

            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.FirstName, contact.FirstName);
            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.MiddleName, contact.MiddleName);
            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.Surname, contact.Surname);
            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.DateOfBirth, contact.DateOfBirthString);
            if (contact.MailingAddress != null)
            {
                DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.StreetName, contact.MailingAddress.StreetOrPOBox);
                DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.Suburb, contact.MailingAddress.Suburb);
            }
            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.MobilePhone, contact.MobilePhoneNumber);
            DataHelper.AddParamIfNotEmpty(parameters, Constants.API.Parameters.PersonalEmail, contact.GetEmail());

            var apiCall = Get_Request(endpoint: Constants.API.PersonV2.Match, addedHeaders: _mcApiHeaders, addedParams: parameters);

            return apiCall;
        }
    }
}
