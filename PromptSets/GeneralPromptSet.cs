namespace sts_ai_support.PromptSets
{
    public class GeneralPromptSet : QuestionAnswerResponsePromptSet
    {
        public override string SystemPrompt => """
            You are an expert in educational technology and IT infrastructure for schools and colleges.
            You work as a content designer at the UK Department for Education, and your role is to help educational institutions meeting digital and technology standards.
            You do this by creating collections of questions and answers which help educational institutions understand their current level of digital maturity,
            and to guide them to implement technology standards effectively.
            The questionnaires are to be designed to be completed by head teachers and other members of the senior leadership team.
            The questions should be posed in such a way that they help staff to learn and understand: 
            - that they should have one or more staff responsible for delivering and maintaining the plans, processes, registers,
              risks/risk profiles, strategies, and tasks associated with the standards;
            - that they should have relevant policies in place to support those tasks;
            - how often the policies and tasks should be reviewed;
            - what considerations should be made when doing so (e.g. risk profiles, tech usage);
            - what other organisations could be used for reference (e.g. IWF, CTIRU)
            """;

        protected override string UserPromptTemplate => """
            Here is a standard titled '{{title}}':

            ```
            {{standard}}
            ```

            Please analyze this content and provide questions and possible answers. Each answer should have one or more associated recommendations.
            The questions and recommendations are for non-technical users and should avoid the use of jargon, have acronyms expanded, and be accessible to the lay person.

            Each question should:
            - focus on practical, actionable aspects
            - consider both technical and organizational factors
            - be clear and concise

            Your response should consist of only JSON, in the following format:
            
            ```
            {
              "questions": [
                {
                  "text": "",
                  "answers": [
                    {
                      "text": "",
                      "recommendations": [""]
                    }
                  ]
                }
              ]
            }
            ```

            Note: The questions should be focused on the non-technical aspects of the guidance content.
            """;

        public GeneralPromptSet(string title, string standard)
            : base (title, standard)
        {
        }
    }
}
