using Newtonsoft.Json;


namespace Rac.TestAutomation.Common.API
{
    public class API_InsuranceCompanies
    {
        [JsonProperty("externalContactNumber")]
        public string ExternalContactNumber { get; set; }       

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
