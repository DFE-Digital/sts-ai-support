using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace sts_ai_support.Models
{
    public class RequestModel
    {
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 4096;

        [JsonPropertyName("messages")]
        public required IEnumerable<RequestChatMessageModel> Messages { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 1;
    }
}
