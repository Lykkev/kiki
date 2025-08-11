using Newtonsoft.Json;

namespace Kick_ChatBOT.Models
{
    public class UserPaths
    {
        [JsonProperty("kicks")] public string Kicks { get; set; }
        [JsonProperty("proxies")] public string Proxies { get; set; }
        [JsonProperty("messages")] public string Messages { get; set; }
        [JsonProperty("config")] public string Config { get; set; }
    }
}

