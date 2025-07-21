using System.Text.Json.Serialization;

namespace sts_ai_support.Models
{
    public class RequestModel
    {
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 8192;

        [JsonPropertyName("messages")]
        public required IEnumerable<RequestChatMessageModel> Messages { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 1;
    }
}
