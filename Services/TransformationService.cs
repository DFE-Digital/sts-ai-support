using System.Text.RegularExpressions;
using HtmlAgilityPack;
using sts_ai_support.Models;

namespace sts_ai_support.Services
{
    public class TransformationService
    {
        private const string ScriptRegex = @"<script\b[^<]*(?:(?!</script>)<[^<]*)*</script>";
        private const string StyleRegex = @"<style\b[^<]*(?:(?!</style>)<[^<]*)*</style>";
        private const string DataRegex = @"\s+data-[^=]+=[""'][^""']*[""']";
        private const string EmptyParagraphRegex = @"<p>\s*</p>";

        public string TransformContent(StandardModel section)
        {
            var content = section.Content;

            // Remove non-text tags
            content = Regex.Replace(content, ScriptRegex, string.Empty);
            content = Regex.Replace(content, StyleRegex, string.Empty);
            content = Regex.Replace(content, DataRegex, string.Empty);
            content = Regex.Replace(content, EmptyParagraphRegex, string.Empty);

            var document = new HtmlDocument();
            document.LoadHtml(content);

            var replacements = new Dictionary<string, string>();
            string cleanText = ExtractText(document.DocumentNode, replacements);

            // Remove repeat newlines
            while (cleanText.Contains(Environment.NewLine + Environment.NewLine))
            {
                cleanText = cleanText.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }

            /*
            // Replace abbreviations with their full text
            foreach (var replacement in replacements)
            {
                cleanText = cleanText.Replace(replacement.Key, replacement.Value);
            }
            //*/

            return cleanText;
        }

        static string ExtractText(HtmlNode node, Dictionary<string, string> replacements)
        {
            var innerText = node.InnerText.Trim();
            var result = string.Empty;

            if (node.NodeType.Equals(HtmlNodeType.Text) ||
                node.NodeType.Equals(HtmlNodeType.Document) ||
                node.Name.Equals("div"))
            {
                result = string.Empty;
            }

            else if (Regex.IsMatch(node.Name, @"h\d"))
            {
                var headerLevel = int.Parse(node.Name.AsSpan(1));
                result = new string('#', headerLevel) + " " + innerText;
            }

            else if (node.Name.Equals("a"))
            {
                if (node.HasAttributes && node.Attributes.Any(a => a.Name == "href"))
                {
                    innerText += $" ({node.Attributes["href"].Value})";
                }

                result = innerText;
            }

            else if (node.Name.Equals("abbr"))
            {
                if (node.HasAttributes &&
                    node.Attributes.Any(a => a.Name == "title") &&
                    !replacements.ContainsKey(node.InnerText))
                {
                    replacements.Add(node.InnerText, $"{node.InnerText} ({node.Attributes["title"].Value})");
                }
            }

            else if (node.Name.Equals("li"))
            {
                result = "- " + innerText;
            }

            else if (node.Name.Equals("ul"))
            {
                // Handled by recursion below
                result = string.Empty;
            }

            else
            {
                result = innerText;
            }

            // Recursively process child nodes
            foreach (var child in node.ChildNodes)
            {
                result += ExtractText(child, replacements);
            }

            return result.Trim() + Environment.NewLine;
        }
    }
}
