using System.Text.Json.Serialization;

namespace sts_ai_support.Models
{
    public class AnswerModel
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("recommendations")]
        public IEnumerable<string> Recommendations { get; set; }
    }
}
