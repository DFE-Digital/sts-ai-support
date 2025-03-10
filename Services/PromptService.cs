using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public class PromptService : IPromptService
    {
        public IList<PromptSet> GetQuestionAnswerResponsePromptSets(string title, string standard)
        {
            var promptSets = new List<PromptSet>
            {
                new GeneralPromptSet(title, standard),
                new TechnicalPromptSet(title, standard)
            };

            return promptSets;
        }
    }
}
