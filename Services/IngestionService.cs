using System.Text.RegularExpressions;
using sts_ai_support.Helpers;
using sts_ai_support.Models;

namespace sts_ai_support.Services
{
    public class IngestionService
    {
        public const string BASE_URL = "";

        private readonly HttpClient _httpClient;
        
        private const string StandardsRegex = @"<ul[^>]*class=""[^""]*gem-c-document-list[^""]*""[^>]*>(.*?)</ul>";
        private const string StandardRegex = @"<div class=""gem-c-document-list__item-title"">\s*<a[^>]*href=""([^""]*)""[^>]*>(.*?)</a>";
        private const string SectionsRegex = @"class=""govuk-accordion__section-header"">\s*<h2[^>]*class=""govuk-accordion__section-heading""[^>]*>\s*<span[^>]*class=""govuk-accordion__section-button""[^>]*>(.*?)</span>";
        private const string SectionRegex = @"<div[^>]*class=""[^""]*govspeak[^""]*""[^>]*>(.*?)</div>";

        private const string SectionsContentMarker = "Show all sections";

        private IList<Standard> _standards;

        public IngestionService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Constants.Domain);

            _standards = new List<Standard>();
        }

        public async Task Ingest()
        {
            var parseStandard = (GroupCollection group) => new Standard(group);
            var parseSectionTitle = (GroupCollection group) => group.Values.Skip(1).First().Value.Trim();

            var landingPageHtml = await GetHtml(Constants.SectionsSlug);
            var sectionsHtml = GetMatch(landingPageHtml, StandardsRegex);
            var standards = GetMatches(sectionsHtml, StandardRegex, parseStandard);

            foreach (var standard in standards)
            {
                var sectionHtml = await GetHtml(standard.Slug);
                var contentStart = sectionHtml.IndexOf(SectionsContentMarker);
                var contentHtml = sectionHtml.Substring(contentStart + SectionsContentMarker.Length);

                var sections = new List<Section>();
                var sectionTitles = GetMatches(contentHtml, SectionsRegex, parseSectionTitle);
                foreach(var sectionTitle in sectionTitles)
                {
                    var sectionStart = contentHtml.IndexOf(sectionTitle);
                    var sectionContent = contentHtml.Substring(sectionStart);
                    var content = GetMatch(sectionContent, SectionRegex);
                    
                    sections.Add(new Section(sectionTitle, content));
                }

                standard.Sections = sections;
                _standards.Add(standard);
            }

            Console.WriteLine("Ingestion complete");
        }

        private async Task<string> GetHtml(string slug)
        {
            var response = await _httpClient.GetAsync(slug);
            return await response.Content.ReadAsStringAsync();
        }

        private string GetMatch(string value, string expression)
        {
            var regex = new Regex(expression, RegexOptions.Singleline);
            var match = regex.Match(value);
            if (!match.Success)
            {
                throw new InvalidDataException("Regular expression returned no match");
            }

            return match.Captures.First().Value.Trim();
        }

        private IEnumerable<T> GetMatches<T>(string value, string expression, Func<GroupCollection, T> parseFunction)
        {
            var regex = new Regex(expression, RegexOptions.Singleline);
            var match = regex.Matches(value);
            if (match == null || match.All(m => !m.Success))
            {
                throw new InvalidDataException("Regular expression returned no matches");
            }

            return match
                .Where(m => m.Success)
                .Select(m => m.Groups)
                .Select(parseFunction);
        }
    }
}
