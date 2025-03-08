using System.Text.RegularExpressions;

namespace sts_ai_support.Helpers
{
    public static class RegexHelpers
    {
        public static string GetMatch(string value, string expression)
        {
            var regex = new Regex(expression, RegexOptions.Singleline);
            var match = regex.Match(value);
            if (!match.Success)
            {
                throw new InvalidDataException("Regular expression returned no match");
            }

            return match.Captures.First().Value.Trim();
        }

        public static IEnumerable<T> GetMatches<T>(string value, string expression, Func<GroupCollection, T> parseFunction, RegexOptions regexOptions = RegexOptions.Singleline)
        {
            var regex = new Regex(expression, regexOptions);
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
