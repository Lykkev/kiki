using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kick_ChatBOT.Models
{
    public class DelayRange
    {
        [JsonProperty("min")] public int Min { get; set; }
        [JsonProperty("max")] public int Max { get; set; }
    }

    public class Delays
    {
        [JsonProperty("initial_wait")] public DelayRange InitialWait { get; set; }
        [JsonProperty("between_messages")] public DelayRange BetweenMessages { get; set; }
        [JsonProperty("typing_simulation")] public DelayRange TypingSimulation { get; set; }
        [JsonProperty("between_accounts")] public int BetweenAccounts { get; set; }
        [JsonProperty("batch_pause")] public DelayRange BatchPause { get; set; }
    }

    public class Behavior
    {
        [JsonProperty("max_concurrent_bots")] public int MaxConcurrentBots { get; set; }
        [JsonProperty("messages_per_session")] public int MessagesPerSession { get; set; }
        [JsonProperty("add_human_variations")] public bool AddHumanVariations { get; set; }
        [JsonProperty("randomize_order")] public bool RandomizeOrder { get; set; }
        [JsonProperty("only_emotes")] public bool OnlyEmotes { get; set; }
    }

    public class Safety
    {
        [JsonProperty("respect_rate_limits")] public bool RespectRateLimits { get; set; }
        [JsonProperty("auto_retry_on_429")] public bool AutoRetryOn429 { get; set; }
        [JsonProperty("max_retries_per_account")] public int MaxRetriesPerAccount { get; set; }
        [JsonProperty("stop_on_token_error")] public bool StopOnTokenError { get; set; }
    }

    public class ProxySettings
    {
        [JsonProperty("enabled")] public bool Enabled { get; set; }
        [JsonProperty("auto_assign")] public bool AutoAssign { get; set; }
        [JsonProperty("show_proxy_info")] public bool ShowProxyInfo { get; set; }
        [JsonProperty("fallback_to_direct")] public bool FallbackToDirect { get; set; }
        [JsonProperty("max_retries_per_proxy")] public int MaxRetriesPerProxy { get; set; }
    }

    public class AppConfig
    {
        [JsonProperty("delays")] public Delays Delays { get; set; }
        [JsonProperty("behavior")] public Behavior Behavior { get; set; }
        [JsonProperty("safety")] public Safety Safety { get; set; }
        [JsonProperty("proxy")] public ProxySettings Proxy { get; set; }
    }
}

