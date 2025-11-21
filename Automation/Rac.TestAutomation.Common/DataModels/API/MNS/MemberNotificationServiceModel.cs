using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rac.TestAutomation.Common.API
{
    public class Entity
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }
    }

    public class MemberEvent
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("entities")]
        public List<Entity> Entities { get; set; }
    }

}
