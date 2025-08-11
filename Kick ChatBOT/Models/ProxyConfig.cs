using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kick_ChatBOT.Models
{
    public class ProxyEntry
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("host")] public string Host { get; set; }
        [JsonProperty("port")] public int Port { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
        [JsonProperty("active")] public bool Active { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("last_used")] public long? LastUsed { get; set; }
        [JsonProperty("error_count")] public int ErrorCount { get; set; }
    }

    public class ProxiesFile
    {
        [JsonProperty("proxies")] public List<ProxyEntry> Proxies { get; set; }
        [JsonProperty("settings")] public ProxySettingsFile Settings { get; set; }
    }

    public class ProxySettingsFile
    {
        [JsonProperty("max_error_count")] public int MaxErrorCount { get; set; }
        [JsonProperty("proxy_timeout")] public int ProxyTimeout { get; set; }
        [JsonProperty("rotate_on_error")] public bool RotateOnError { get; set; }
        [JsonProperty("use_random_proxy")] public bool UseRandomProxy { get; set; }
        [JsonProperty("proxy_enabled")] public bool ProxyEnabled { get; set; }
    }
}

