using System.Text.Json.Serialization;

namespace sts_ai_support.Models
{
    public class LlmResponseModel
    {
        [JsonPropertyName("choices")]
        public required IEnumerable<LlmResponseChoice> Choices { get; set; }
    }

    public class LlmResponseChoice
    {
        [JsonPropertyName("message")]
        public required LlmResponseMessage Message { get; set; }
    }

    public class LlmResponseMessage
    {
        [JsonPropertyName("content")]
        public required string Content { get; set; }
    }
}
