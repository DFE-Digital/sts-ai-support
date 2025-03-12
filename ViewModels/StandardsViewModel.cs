using sts_ai_support.Models;

namespace sts_ai_support.ViewModels
{
    public class StandardsViewModel
    {
        public Guid TopicId { get; set; }
        public required IEnumerable<StandardModel> Standards { get; set; }
    }
}
