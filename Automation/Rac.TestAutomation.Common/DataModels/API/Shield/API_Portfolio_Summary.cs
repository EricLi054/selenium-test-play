
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Rac.TestAutomation.Common.API
{

    public class Contacts
    {
        [JsonProperty("contactId")]
        public int contactId { get; set; }

        [JsonProperty("contactExternalNumber")]
        public string contactExternalNumber { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("policyDetails")]
        public List<PolicyDetail> policyDetails { get; set; }
    }

    public class Cover
    {
        [JsonProperty("coverType")]
        public string coverType { get; set; }

        [JsonProperty("coverTypeDescription")]
        public string coverTypeDescription { get; set; }

        [JsonProperty("updateVersion")]
        public int updateVersion { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }
    }

    public class HomeAsset
    {
        [JsonProperty("streetName")]
        public string streetName { get; set; }

        [JsonProperty("gnafPid")]
        public string gnafPid { get; set; }

        [JsonProperty("houseNumber")]
        public string houseNumber { get; set; }

        [JsonProperty("suburb")]
        public string suburb { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }
    }

    public class MotorAsset
    {
        [JsonProperty("modelDescription")]
        public string modelDescription { get; set; }

        [JsonProperty("updateVersion")]
        public int updateVersion { get; set; }

        [JsonProperty("year")]
        public int year { get; set; }

        [JsonProperty("registrationNumber")]
        public string registrationNumber { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("manufacturer")]
        public string manufacturer { get; set; }
    }

    public class PolicyDetail
    {
        [JsonProperty("overdueAmount")]
        public double overdueAmount { get; set; }

        [JsonProperty("updateVersion")]
        public int updateVersion { get; set; }

        [JsonProperty("policyNumber")]
        public string policyNumber { get; set; }

        [JsonProperty("policyStartDate")]
        public DateTime policyStartDate { get; set; }

        [JsonProperty("homeAsset")]
        public HomeAsset homeAsset { get; set; }

        [JsonProperty("cover")]
        public List<Cover> cover { get; set; }

        [JsonProperty("policyContactRole")]
        public string policyContactRole { get; set; }

        [JsonProperty("policyType")]
        public PolicyType policyType { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("motorAsset")]
        public MotorAsset motorAsset { get; set; }
    }

    public class PolicyType
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("productType")]
        public string productType { get; set; }
    }

    public class GetPortfolioSummary_Response
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("contacts")]
        public List<Contacts> contacts { get; set; }
    }
}

