using System.Text.Json.Serialization;

namespace sts_ai_support.ViewModels
{
    public class StandardsResponseViewModel
    {
        [JsonPropertyName("standardsHtml")]
        public required string StandardsHtml { get; set; }

        [JsonPropertyName("sourcesHtml")]
        public required string SourcesHtml { get; set; }
    }
}
