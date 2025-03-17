namespace sts_ai_support.PromptSets
{
    public abstract class PromptSet
    {
        public abstract string SystemPrompt { get; set; }
        protected abstract string UserPromptTemplate { get; }
        public string UserPrompt => BuildUserPrompt();
        public string? Response { get; set; }
        public abstract string? ChainedPrompt { get; }

        protected abstract IDictionary<string, string> GetPromptValues();
        protected abstract void ValidatePromptValues(IDictionary<string, string> promptValues);

        protected string BuildUserPrompt()
        {
            var promptValues = GetPromptValues();
            ValidatePromptValues(promptValues);

            var prompt = UserPromptTemplate;
            foreach (var (key, value) in promptValues)
            {
                prompt = prompt.Replace(key, value);
            }

            return prompt.Trim();
        }
    }
}
