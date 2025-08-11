using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kick_ChatBOT.Models
{
    public class Greetings
    {
        [JsonProperty("greetings")] public List<string> Greetings { get; set; }
        [JsonProperty("farewells")] public List<string> Farewells { get; set; }
    }
}
