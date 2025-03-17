using System.Text.Json.Serialization;
using sts_ai_support.Models;

namespace sts_ai_support.ViewModels
{
    public class QARResponseViewModel
    {
        [JsonPropertyName("questions")]
        public required IEnumerable<QuestionModel> Questions { get; set; }
    }
}
