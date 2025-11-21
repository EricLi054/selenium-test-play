using System.Collections.Generic;
using Newtonsoft.Json;


namespace Rac.TestAutomation.Common.API
{
    public class SearchServiceProviders
    {
        [JsonProperty("suburb")]
        public string Suburb { get; set; }

        [JsonProperty("serviceTypes")]
        public List<ServiceTypes> ServiceTypes { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("claimNumber")]
        public string ClaimNumber { get; set; }
    }

    public class ServiceProvider
    {
        [JsonProperty("isRapidRepairer")]
        public bool IsRapidRepairer { get; set; }

        [JsonProperty("repairerEmail")]
        public string RepairerEmail { get; set; }

        [JsonProperty("repairerName")]
        public string RepairerName { get; set; }

        [JsonProperty("repairerAddress")]
        public string RepairerAddress { get; set; }

        [JsonProperty("listOrder")]
        public string ListOrder { get; set; }

        [JsonProperty("detailedRepairerAddress")]
        public Address DetailRepairerAddress { get; set; }

        [JsonProperty("contactExternalNumber")]
        public string ContactExternalNumber { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("repairerPhoneNumber")]
        public string RepairerPhoneNumber { get; set; }

        [JsonProperty("repairerFaxNumber")]
        public string RepairerFaxNumber { get; set; }

        [JsonProperty("isReadyDrive")]
        public bool IsReadyDrive { get; set; }
    }

    public class ServiceTypes
    {
        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("serviceProviders")]
        public List<ServiceProvider> ServiceProviders { get; set; }
    }
}
