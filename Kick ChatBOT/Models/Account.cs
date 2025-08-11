using System;
using Newtonsoft.Json;

namespace Kick_ChatBOT.Models
{
    public class Account
    {
        [JsonProperty("token")] public string Token { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("user_id")] public long UserId { get; set; }
        [JsonProperty("verified")] public bool Verified { get; set; }
        [JsonProperty("personality")] public string Personality { get; set; }
        [JsonProperty("last_message_time")] public double LastMessageTime { get; set; }
        [JsonProperty("messages_sent")] public int MessagesSent { get; set; }
        [JsonProperty("session_start")] public double SessionStart { get; set; }
        [JsonProperty("created_at")] public double CreatedAt { get; set; }
        [JsonProperty("real")] public bool Real { get; set; }
        [JsonProperty("manual")] public bool Manual { get; set; }
        [JsonProperty("added_at")] public string AddedAt { get; set; }
        [JsonProperty("demo")] public bool? Demo { get; set; }
    }
}

