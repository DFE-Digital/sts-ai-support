using System.Text.Json.Serialization;
using sts_ai_support.Models;

namespace sts_ai_support.ViewModels
{
    public class QARResponseViewModel
    {
        public string? StandardTitle { get; set; }
        [JsonPropertyName("questions")]
        public required IEnumerable<QuestionModel> Questions { get; set; }
    }
}
