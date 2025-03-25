namespace sts_ai_support.ViewModels
{
    public class StandardViewModel
    {
        public required Guid TopicId { get; set; }
        public required Guid StandardId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}
