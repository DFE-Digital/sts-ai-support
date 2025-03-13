using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public interface IPromptService
    {
        public PromptSet GetStandardsPromptSet(string prompt);
        public IList<PromptSet> GetQuestionAnswerResponsePromptSets(string title, string standard);
    }
}