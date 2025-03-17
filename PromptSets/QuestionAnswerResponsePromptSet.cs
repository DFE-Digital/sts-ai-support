namespace sts_ai_support.PromptSets
{
    public abstract class QuestionAnswerResponsePromptSet : PromptSet
    {
        private string _title;
        private string _titleKey = "{{title}}";
        private string _standard;
        private string _standardKey = "{{standard}}";

        public override string ChainedPrompt => """
Compare your responses to the information provided in my first message and reflect on whether they cover all the of the guidance sufficiently.
Where you identify areas of improvement, adapt your responses accordingly.

Ensure that formatting, structure, tone, and language is refined to match the existing standards and GDS/accessibility requirements.

Ensure that your response does not include contractions, complex language, or phrasing choices that are not helpful when communicating with users.

Do not overcomplicate and increase the formality of the writing to a level that is unnecessary and detrimental.

Return a full set of amended questions/answers/recommendations to replace your first attempt.
""";

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
