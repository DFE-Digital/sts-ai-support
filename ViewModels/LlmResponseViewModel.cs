using System.Text.Json.Serialization;
using sts_ai_support.Models;

namespace sts_ai_support.ViewModels
{
    public class QAResponseViewModel
    {
        [JsonPropertyName("questions")]
        public IEnumerable<QuestionModel> Questions { get; set; }
    }
}
