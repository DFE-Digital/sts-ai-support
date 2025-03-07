using System.Text.RegularExpressions;

namespace sts_ai_support.Models
{
    public class Section
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public Section(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
