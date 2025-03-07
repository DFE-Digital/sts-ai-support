using System.Text.RegularExpressions;

namespace sts_ai_support.Models
{
    public class Standard
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public IEnumerable<Section> Sections { get; set; }

        public Standard(GroupCollection groups)
        {
            Slug = groups.Values.Skip(1).First().Value.Trim();
            Title = groups.Values.Skip(2).First().Value.Trim();
            Sections = [];
        }
    }
}
