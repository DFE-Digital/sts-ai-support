using sts_ai_support.Models;

namespace sts_ai_support.Services
{
    public interface ITransformationService
    {
        public string TransformContent(StandardModel section);
    }
}