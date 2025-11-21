using Newtonsoft.Json;

namespace Rac.TestAutomation.Common.API
{
    public class ShieldErrorMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
