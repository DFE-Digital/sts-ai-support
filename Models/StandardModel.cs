using System.Text.RegularExpressions;

namespace sts_ai_support.Models
{
    public class StandardModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Content { get; set; }

        public StandardModel(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
