using System.Text.RegularExpressions;
using sts_ai_support.Helpers;
using sts_ai_support.Models;

namespace sts_ai_support.Services
{
    public class IngestionService : IIngestionService
    {
        public bool LoadingComplete => _topics.Any();
        public IEnumerable<TopicModel> Topics => LoadingComplete
            ? _topics
            : [];
        
        private readonly HttpClient _httpClient;

        private const string TopicsRegex = @"<ul[^>]*class=""[^""]*gem-c-document-list[^""]*""[^>]*>(.*?)</ul>";
        private const string TopicRegex = @"<div class=""gem-c-document-list__item-title"">\s*<a[^>]*href=""([^""]*)""[^>]*>(.*?)</a>";
        private const string StandardsRegex = @"class=""govuk-accordion__section-header"">\s*<h2[^>]*class=""govuk-accordion__section-heading""[^>]*>\s*<span[^>]*class=""govuk-accordion__section-button""[^>]*>(.*?)</span>";
        private const string StandardRegex = @"<div[^>]*class=""[^""]*govspeak[^""]*""[^>]*>(.*?)</div>";

        private const string SectionsContentMarker = "Show all sections";

        private IEnumerable<TopicModel> _topics;

        public IngestionService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(Constants.Domain)
            };

            _topics = [];
        }

        public async Task Ingest()
        {
            var landingPageHtml = await GetHtml(Constants.SectionsSlug);
            var topicsHtml = RegexHelpers.GetMatch(landingPageHtml, TopicsRegex);
            _topics = await GetTopics(topicsHtml);
        }

        private async Task<string> GetHtml(string slug)
        {
            var response = await _httpClient.GetAsync(slug);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<IEnumerable<TopicModel>> GetTopics(string html)
        {
            var parseTopic = (GroupCollection group) => new TopicModel(group);

            var topics = new List<TopicModel>();

            var tempTopics = RegexHelpers.GetMatches(html, TopicRegex, parseTopic);
            foreach (var topic in tempTopics)
            {
                var topicHtml = await GetHtml(topic.Slug);
                var contentStart = topicHtml.IndexOf(SectionsContentMarker);
                if (contentStart < 0)
                {
                    throw new InvalidOperationException($"Could not find index of {nameof(SectionsContentMarker)}");
                }

                topicHtml = topicHtml.Substring(contentStart + SectionsContentMarker.Length);
                topic.Standards = GetStandards(topicHtml);
                topics.Add(topic);
            }

            return topics;
        }

        private IEnumerable<StandardModel> GetStandards(string html)
        {
            var parseStandardTitle = (GroupCollection group) => group.Values.Skip(1).First().Value.Trim();

            var standards = new List<StandardModel>();

            var standardTitles = RegexHelpers.GetMatches(html, StandardsRegex, parseStandardTitle);
            foreach (var standardTitle in standardTitles)
            {
                var standardStart = html.IndexOf(standardTitle);
                var htmlContent = html.Substring(standardStart);
                var standardContent = RegexHelpers.GetMatch(htmlContent, StandardRegex);

                standards.Add(new StandardModel(standardTitle, standardContent));
            }

            return standards;
        }
    }
}
