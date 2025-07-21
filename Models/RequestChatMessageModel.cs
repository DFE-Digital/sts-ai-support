using System.Text.Json.Serialization;

namespace sts_ai_support.Models
{
    public class RequestChatMessageModel
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        public RequestChatMessageModel(string content, string role)
        {
            Content = content;
            Role = role;
        }
    }
}
