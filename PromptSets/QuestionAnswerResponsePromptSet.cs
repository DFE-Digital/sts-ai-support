namespace sts_ai_support.PromptSets
{
    public abstract class QuestionAnswerResponsePromptSet : PromptSet
    {
        private string _title;
        private string _titleKey = "{{title}}";
        private string _standard;
        private string _standardKey = "{{standard}}";

        public QuestionAnswerResponsePromptSet(string title, string standard)
        {
            _title = title;
            _standard = standard;
        }

        protected override Dictionary<string, string> GetPromptValues()
        {
            return new Dictionary<string, string>()
            {
                { _titleKey, _title },
                { _standardKey, _standard },
            };
        }

        protected override void ValidatePromptValues(IDictionary<string, string> promptValues)
        {
            if (promptValues.Count() != 2)
            {
                throw new ArgumentException($"A {nameof(QuestionAnswerResponsePromptSet)} user prompt requires two parameters, for '{_titleKey}' and '{_standardKey}'");
            }

            if (!promptValues.ContainsKey(_titleKey))
            {
                throw new ArgumentException($"A {nameof(QuestionAnswerResponsePromptSet)} user prompt requires the key '{_titleKey}'");
            }

            if (string.IsNullOrWhiteSpace(promptValues[_titleKey]))
            {
                throw new ArgumentException($"A {nameof(QuestionAnswerResponsePromptSet)} user prompt requires a non-empty value for the '{_titleKey}' parameter");
            }

            if (!promptValues.ContainsKey(_standardKey))
            {
                throw new ArgumentException($"A {nameof(QuestionAnswerResponsePromptSet)} user prompt requires the key '{_standardKey}'");
            }

            if (string.IsNullOrWhiteSpace(promptValues[_standardKey]))
            {
                throw new ArgumentException($"A {nameof(QuestionAnswerResponsePromptSet)} user prompt requires a non-empty value for the '{_standardKey}' parameter");
            }
        }
    }
}
