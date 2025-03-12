using System.Text.Json.Serialization;

namespace sts_ai_support.Models
{
    public class QuestionModel
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("answers")]
        public IEnumerable<AnswerModel> Answers { get; set; }
    }
}
