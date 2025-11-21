using Newtonsoft.Json;

namespace Rac.TestAutomation.Common
{
    /// <summary>
    /// Model for the JSON encoded into Spark URLs to set the feature toggles.
    /// </summary>
    public class SparkFeatureToggle
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("headerName")]
        public string HeaderName { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
