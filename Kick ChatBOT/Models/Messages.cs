using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kick_ChatBOT.Models
{
    public class Messages
    {
        [JsonProperty("personalities")] public Dictionary<string, List<string>> Personalities { get; set; }
        [JsonProperty("kick_emotes")] public Dictionary<string, List<string>> KickEmotes { get; set; }
        [JsonProperty("emojis")] public Dictionary<string, List<string>> Emojis { get; set; }
        [JsonProperty("punctuation")] public List<string> Punctuation { get; set; }
        [JsonProperty("common_words")] public Dictionary<string, List<string>> CommonWords { get; set; }
    }
}

