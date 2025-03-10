
using sts_ai_support.Models;

namespace sts_ai_support.Services
{
    public interface IIngestionService
    {
        public bool LoadingComplete { get; }
        public IEnumerable<TopicModel> Topics { get; }
        public Task Ingest();
    }
}