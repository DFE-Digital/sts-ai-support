namespace sts_ai_support.PromptSets
{
    public abstract class SimplePromptSet : PromptSet
    {
        private string _prompt;
        private string _promptKey = "{{prompt}}";

        public SimplePromptSet(string prompt)
        {
            _prompt = prompt;
        }

        protected override Dictionary<string, string> GetPromptValues()
        {
            return new Dictionary<string, string>()
            {
                { _promptKey, _prompt }
            };
        }

        protected override void ValidatePromptValues(IDictionary<string, string> promptValues)
        {
            if (promptValues.Count() != 1)
            {
                throw new ArgumentException($"A {nameof(SimplePromptSet)} user prompt requires one parameter, for '{_promptKey}'");
            }

            if (!promptValues.ContainsKey(_promptKey))
            {
                throw new ArgumentException($"A {nameof(SimplePromptSet)} user prompt requires the key '{_promptKey}'");
            }

            if (string.IsNullOrWhiteSpace(promptValues[_promptKey]))
            {
                throw new ArgumentException($"A {nameof(SimplePromptSet)} user prompt requires a non-empty value for the '{_promptKey}' parameter");
            }
        }
    }
}
