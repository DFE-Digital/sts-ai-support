using System.Text.RegularExpressions;

namespace sts_ai_support.Models
{
    public class TopicModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Slug { get; set; }
        public string Title { get; set; }
        public IEnumerable<StandardModel> Standards { get; set; }

        public TopicModel(GroupCollection groups)
        {
            Slug = groups.Values.Skip(1).First().Value.Trim();
            Title = groups.Values.Skip(2).First().Value.Trim();
            Standards = [];
        }
    }
}
