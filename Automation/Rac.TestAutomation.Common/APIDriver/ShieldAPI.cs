using Newtonsoft.Json;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants;

namespace Rac.TestAutomation.Common.APIDriver
{
    public class ShieldAPI : BaseAPI
    {
        private class Constant
        {
            public class Endpoint
            {
                public const string Contacts = "insurance/contacts/api/v1/contacts";
                public const string Quotes   = "insurance/policy/api/v1/quotes";
                public const string Policies = "insurance/policy/api/v1/policies";
                public const string InsuranceCompanies = "insurance/contacts/api/v1/insurance-companies";
                public static string ContactById(string id) => $"{Contacts}?{Parameter.ContactId}={id}";

                public const string PortfolioSummary = "insurance/reference-data/api/v1/pcm/portfolio-summary";

                public static string AddressStatusByGnaf(string gnafId) => $"insurance/reference-data/api/v1/home/address/status?gnafPid={gnafId}";

                public class Claims
                {
                    private const string Base = "insurance/claim/api/v1/";

                    public static string ByClaimId(string id) => $"{Base}claim/{id}";
                    public const string FenceBreakdown = Base + "fenceSettlementBreakdown";
                    public const string SearchServiceProviders = Base + "search-service-providers";

                }
                public class Vehicles
                {
                    private const string Base = "insurance/reference-data/api/v1/motor/vehicles/";
                    public static string ById(string id) => $"{Base}{id}";

                    public class Caravan
                    {
                        public const string Manufacturer = Base + "caravan-manufacturer-years";
                        public const string Search       = Base + "caravans";
                    }
                    public class Motor
                    {
                        public const string Manufacturer = Base + "motor-manufacturer-years";
                        public const string Search       = Base + "general";
                    }
                    public class Motorcycles
                    {
                        public const string Manufacturer = Base + "motorcycle-manufacturer-years";
                        public const string Search       = Base + "motorcycles";
                    }
                }
            }
            public class Parameter
            {
                // Vehicle Search
                public const string ExcludeInvalidVehiclePoints = "excludeInvalidVehiclePoints";
                public const string ExcludeInvalidRateCodes     = "excludeInvalidRateCodes";
                public const string IncludeDiscontinued         = "includeDiscontinued";
                public const string Manufacturer                = "manufacturer";
                public const string SubType    = "vehicleSubType";
                public const string Year       = "year";
                public const string YearFrom   = "yearFrom";
                public const string YearTo     = "yearTo";
                public const string AmountFrom = "amountFrom";
                public const string AmountTo   = "amountTo";

                public class Value
                {
                    public const string CaravanSubType = "C3&vehicleSubType=C5&vehicleSubType=CD";
                }

                // Contacts
                public const string ContactId = "contactId";

                // Address
                public const string GnafId = "gnafPid";
            }
            public class Header
            {
                public const string Key         = "Ocp-Apim-Subscription-Key";
                public const string Environment = "Environment";
            }

            public class ErrorMessage
            {
                public const string UnsupportedType = "Only supporting Motorcycles, Cars and Caravans at this time.";
            }
        }

        private Dictionary<string, string> _shieldApiHeaders;

        public ShieldAPI() : base()
        {
            var config = Config.Get();
            var zone = "sit";
            if (config.Shield.IsUatEnvironment())
            { zone = "uat"; }
            else if (config.Shield.IsDevEnvironment())
            { zone = "dev"; }
            _baseUrl = string.Format($"https://az-api-{zone}.ractest.com.au/");

            _shieldApiHeaders = new Dictionary<string, string>()
            {                
                { Constant.Header.Key,         config.Azure.Shield.APIKey },
                { Constant.Header.Environment, config.Shield.Environment },
            };
        }

        /// <summary>
        /// Wrapper around Shield API for fetching the list of manufacturers and years for
        /// a vehicle type and price range. If a Manufacturer does not have any vehicles for
        /// that type and price range, then it will not be returned.
        /// </summary>
        /// <param name="vehicleType">Type of vehicle, roughly correlates to the insurance product type</param>
        /// <param name="minimumValue">Minimum price range to consider</param>
        /// <param name="maximumValue">Maximum price range to consider</param>
        /// <returns></returns>
        /// <exception cref="DataException">Thrown if a unsupported vehicle type is requested</exception>
        /// <exception cref="HttpRequestException">Thrown if we didn't get a 200 response from Shield</exception>
        public async Task<GetManufacturers_Response> GET_SupportedManufacturersAndYearsAsync(PolicyGeneral.Vehicle vehicleType, int minimumValue = 500, int maximumValue = 200000)
        {
            string endpoint = "";
            var parameters = new Dictionary<string, string>() {
                { Constant.Parameter.AmountFrom, minimumValue.ToString() },
                { Constant.Parameter.AmountTo,   maximumValue.ToString() },
                { Constant.Parameter.ExcludeInvalidRateCodes, "true" },
                { Constant.Parameter.IncludeDiscontinued,     "true" }
            };

            switch (vehicleType)
            {
                case PolicyGeneral.Vehicle.Motorcycle:
                    endpoint = Constant.Endpoint.Vehicles.Motorcycles.Manufacturer;
                    break;
                case PolicyGeneral.Vehicle.Car:
                    endpoint = Constant.Endpoint.Vehicles.Motor.Manufacturer;
                    parameters.Add(Constant.Parameter.ExcludeInvalidVehiclePoints, "true");
                    break;
                case PolicyGeneral.Vehicle.Caravan:
                    parameters.Add(Constant.Parameter.SubType, Constant.Parameter.Value.CaravanSubType);
                    parameters.Add(Constant.Parameter.YearFrom, "1960");
                    endpoint = Constant.Endpoint.Vehicles.Caravan.Manufacturer;
                    break;
                default:
                    throw new DataException(Constant.ErrorMessage.UnsupportedType);
            }

            GetManufacturers_Response response = null;
            string result = null;

            var apiCall = await Get_Request(endpoint: endpoint, addedHeaders: _shieldApiHeaders, addedParams: parameters);

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                result = await apiCall.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<GetManufacturers_Response>(result);
            }
            else
            {
                throw new HttpRequestException($"Failed in communication with Shield API, error: {apiCall.StatusCode} {result}");
            }

            return response;
        }

        /// <summary>
        /// Calls the Shield reference data API V1 (v1/motor/vehicles/general)
        /// which returns the list of vehicles matching the year, price range,
        /// and manufacturer given.
        /// 
        /// If a year of 0 is provided, then year is omitted, meaning that the
        /// vehicles of all years will be returned.
        /// </summary>
        /// <param name="vehicleType">Enumeration indicating the type of vehicle being searched (Car, Caravan, Motorcycle)</param>
        /// <param name="manufacturerCode">Three letter code that represents the manufacturer</param>
        /// <param name="year">If 0, then no year constraint is applied, otherwise restricts search to the specific year given</param>
        /// <param name="minimumValue">minimum desired market value</param>
        /// <param name="maximumValue">maximum desired market value</param>
        /// <returns>The full API response from Shield.</returns>
        /// <exception cref="InvalidDataException">If a requested vehicle type is not supported</exception>
        /// <exception cref="HttpRequestException">If we did not get a 200 response back from Shield</exception>
        public async Task<GetVehicle_Response> GET_InsurableModelsForManufacturerAndYear(Constants.PolicyGeneral.Vehicle vehicleType, string manufacturerCode, int year, int minimumValue = 500, int maximumValue = 200000)
        {
            string endpoint = "";
            var parameters = new Dictionary<string, string>() {
                { Constant.Parameter.AmountFrom,   minimumValue.ToString() },
                { Constant.Parameter.AmountTo,     maximumValue.ToString() },
                { Constant.Parameter.Manufacturer, manufacturerCode },
                { Constant.Parameter.ExcludeInvalidRateCodes, "true" }
            };

            if (year > 0)
            { parameters.Add(Constant.Parameter.Year, year.ToString()); }

            switch (vehicleType)
            {
                case PolicyGeneral.Vehicle.Motorcycle:
                    endpoint = Constant.Endpoint.Vehicles.Motorcycles.Search;
                    break;
                case PolicyGeneral.Vehicle.Car:
                    endpoint = Constant.Endpoint.Vehicles.Motor.Search;
                    parameters.Add(Constant.Parameter.ExcludeInvalidVehiclePoints, "true");
                    break;
                case PolicyGeneral.Vehicle.Caravan:
                    endpoint = Constant.Endpoint.Vehicles.Caravan.Search;
                    parameters.Add(Constant.Parameter.IncludeDiscontinued, "true");
                    parameters.Add(Constant.Parameter.SubType, Constant.Parameter.Value.CaravanSubType);
                    break;
                default:
                    throw new DataException(Constant.ErrorMessage.UnsupportedType);
            }

            GetVehicle_Response response = null;
            string result = null;

            var apiCall = await Get_Request(endpoint: endpoint, addedHeaders: _shieldApiHeaders, addedParams: parameters);

            if (apiCall.StatusCode == HttpStatusCode.OK)
            {
                result = await apiCall.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<GetVehicle_Response>(result);
            }
            else
            {
                throw new HttpRequestException($"Failed in communication with Shield API, error: {apiCall.StatusCode} {result}");
            }

            return response;
        }

        /// <summary>
        /// Create provided contact in Shield. Returns the contact
        /// identifier if successful.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>contact id, or NULL if failed to create.</returns>
        public async Task<string> POST_CreateContact(Contact contact)
        {
            Contact response;
            string result = null;
            string body = JsonConvert.SerializeObject(contact);

            var apiCall = await Post_Request(endpoint: Constant.Endpoint.Contacts, body: body, addedHeaders: _shieldApiHeaders);

            if (apiCall.StatusCode != HttpStatusCode.OK)
            {
                Reporting.LogAsyncTask($"Failed in communication with Shield API, error: {apiCall.StatusCode}");
                return null;
            }

            result = await apiCall.Content.ReadAsStringAsync();
            response = JsonConvert.DeserializeObject<Contact>(result);

            return response.Id;
        }

        /// <summary>
        /// Get the quote details (such as Premium Details, Motorcycle Details and Cover Details)
        /// for a given quote number
        /// </summary>
        /// <param name="quoteNumber"></param>
        /// <returns>Quote details from Shield API</returns>
        public async Task<GetQuotePolicy_Response> GET_QuoteDetails(string quoteNumber)
        {
            var apiQuoteResponse = await MakeApiCall($"{Constant.Endpoint.Quotes}/{quoteNumber}");

            return JsonConvert.DeserializeObject<GetQuotePolicy_Response>(await apiQuoteResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Get the details (such as premium, asset information and Cover Details)
        /// for a given policy number
        /// </summary>
        /// <param name="quoteNumber"></param>
        /// <returns>Quote details from Shield API</returns>
        public async Task<GetQuotePolicy_Response> GET_Policy(string policyNumber)
        {
            var apiQuoteResponse = await MakeApiCall($"{Constant.Endpoint.Policies}/{policyNumber}");

            return JsonConvert.DeserializeObject<GetQuotePolicy_Response>(await apiQuoteResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Get the contact details (such as FirstName, Surname and Gender)
        /// for a given contact id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns>Contact details from Shield API</returns>
        public async Task<Contact> GET_ContactDetailsViaContactId(string contactId)
        {
            var apiContactResponse = await MakeApiCall(Constant.Endpoint.ContactById(contactId));

            return JsonConvert.DeserializeObject<Contact>(await apiContactResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Get the contact details (such as FirstName, Surname and Gender)
        /// for a given external contact number (cn_contact.external_contact_number)
        /// </summary>
        /// <param name="externalContactNumber">The External Contact Number related to a Contact in  Shield.</param>
        /// <returns>Contact details from Shield API</returns>
        public async Task<Contact> GET_ContactDetailsViaExternalContactNumber(string externalContactNumber)
        {
            var apiContactResponse = await MakeApiCall($"{Constant.Endpoint.Contacts}/{externalContactNumber}");

            return JsonConvert.DeserializeObject<Contact>(await apiContactResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Get the vehicle details (such as Make, Model and Year)
        /// for a given vehicle id
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>Vehicle details from Shield API</returns>
        public async Task<GetVehicle_Response> GET_VehicleDetails(string vehicleId)
        {
            var apiVehicleResponse = await MakeApiCall(Constant.Endpoint.Vehicles.ById(vehicleId));

            return JsonConvert.DeserializeObject<GetVehicle_Response>(await apiVehicleResponse.Content.ReadAsStringAsync());
        }


        /// <summary>
        /// Get all the policy details for given contact id
        /// </summary>
        /// <param name="contactIds"></param>
        /// <returns>Vehicle details from Shield API</returns>
        public async Task<GetPortfolioSummary_Response> GET_PortfolioSummary(List<string> contactIds)
        {
            var portfolioUri = Constant.Endpoint.PortfolioSummary;
            for(int i = 0; i < contactIds.Count; i++)
            {
                if (i == 0)
                { portfolioUri = $"{portfolioUri}?contactId={contactIds[i]}"; }
                else
                { portfolioUri = $"{portfolioUri}&contactId={contactIds[i]}"; }
            }
            var apiPortfolioResponse = await MakeApiCall(portfolioUri);

            return JsonConvert.DeserializeObject<GetPortfolioSummary_Response>(await apiPortfolioResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Retrieves Shield's status indicators for a insurable home address by
        /// GNAF. The GNAF is a string that should begin with "GAWA_" (WA addresses
        /// only) followed by numbers. We get this from QAS validated addresses.
        /// </summary>
        public async Task<ApiAddressStatus> GET_HomeAddressStatus(string gnafId)
        {
            var apiAddressStatusResponse = await MakeApiCall(Constant.Endpoint.AddressStatusByGnaf(gnafId));

            return JsonConvert.DeserializeObject<ApiAddressStatus>(await apiAddressStatusResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Make a GET Api call for a given API endpoint
        /// </summary>
        /// <param name="apiName"></param>
        /// <returns>The Http Response Message from the API</returns>
        private async Task<HttpResponseMessage> MakeApiCall(string apiName)
        {
            var apiResponse = new HttpResponseMessage();

            try
            {
                apiResponse = await Get_Request(endpoint: apiName, addedHeaders: _shieldApiHeaders);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException)
            {
                Reporting.Log(ex.Message);
            }
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var result = await apiResponse.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ShieldErrorMessage>(result);
                throw new ShieldApiException($"Request to {apiName} received a 400 error because of: {error.Message}");

            }
            else if (apiResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new ShieldApiException(apiResponse.StatusCode.ToString());
            }                

            return apiResponse;
        }


        /// <summary>
        /// Get the Fence settlement cost breakdown
        /// for a given claim number
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns>Fence settlement cost from shield</returns>
        public async Task<GetFenceSettlementBreakdownCost_Response> GET_FenceSettlementBreakdown(string claimNumber)
        {
            var apiFenceSettlementResponse = await MakeApiCall($"{Constant.Endpoint.Claims.FenceBreakdown}?claimNumber={claimNumber}");

            return JsonConvert.DeserializeObject<GetFenceSettlementBreakdownCost_Response>(await apiFenceSettlementResponse.Content.ReadAsStringAsync());
        }


        /// <summary>
        /// Get the basic claim details
        /// for a given claim number
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns>Claim Details</returns>
        public async Task<GetClaimResponse> GET_ClaimDetails(string claimNumber)
        {
            var apiClaimResponse = await MakeApiCall(Constant.Endpoint.Claims.ByClaimId(claimNumber));

            return JsonConvert.DeserializeObject<GetClaimResponse>(await apiClaimResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Get the service providers details
        /// for a given claim number and suburb
        /// </summary>
        /// <exception cref="HttpRequestException">Any exception that occurs is re-wrapped as a HTTPRequestException</exception>
        public async Task<SearchServiceProviders> POST_SearchServiceProviders(string claimNumber, string suburb)
        {
            var requestBody = new SearchServiceProvidersRequestBody
            {
                claimNumber = claimNumber,
                suburb = suburb
            };

            string body = JsonConvert.SerializeObject(requestBody);
            
            SearchServiceProviders response;

            try
            {
                var apiResponse = await Post_Request(endpoint: Constant.Endpoint.Claims.SearchServiceProviders, body: body, addedHeaders: _shieldApiHeaders);
                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    var result = await apiResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<SearchServiceProviders>(result);
                }
                else
                {
                    throw new ShieldApiException(apiResponse.StatusCode.ToString());
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is HttpRequestException)
            {
                throw new HttpRequestException(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Returns the list of insurance companies       
        /// </summary>
        public async Task<List<API_InsuranceCompanies>> GET_InsuranceCompanies()
        {
            var apiContactResponse = await MakeApiCall(Constant.Endpoint.InsuranceCompanies);

            return JsonConvert.DeserializeObject<List<API_InsuranceCompanies>>(await apiContactResponse.Content.ReadAsStringAsync());
        }

        private class SearchServiceProvidersRequestBody
        {
            public string claimNumber { get; set; }
            public string suburb { get; set; }            
        }
    }    
}
    

