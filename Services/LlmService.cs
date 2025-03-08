using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public class LlmService
    {
        private const string AzureOpenAiApiKey = "AzureOpenAI:ApiKey";
        private const string AzureOpenAiDeploymentName = "AzureOpenAI:DeploymentName";
        private const string AzureOpenAiEndpoint = "AzureOpenAI:Endpoint";

        private AzureKeyCredential _apiKey;
        private string _deploymentName;
        private Uri _endpoint;

        private AzureOpenAIClient _openAiClient;

        public LlmService(IConfiguration configuration)
        {
            var apiKey = configuration[AzureOpenAiApiKey] ?? throw new ArgumentNullException(nameof(AzureOpenAiApiKey));
            var deploymentName = configuration[AzureOpenAiDeploymentName] ?? throw new ArgumentNullException(nameof(AzureOpenAiDeploymentName));
            var endpoint = configuration[AzureOpenAiEndpoint] ?? throw new ArgumentNullException(nameof(AzureOpenAiEndpoint));

            _apiKey = new AzureKeyCredential(apiKey);
            _deploymentName = deploymentName;
            _endpoint = new Uri(endpoint);

            _openAiClient = new(_endpoint, _apiKey);
        }

        public async Task<string> SendRequest(IEnumerable<ChatMessage> messages, ChatCompletionOptions completionOptions)
        {
            // Completion without streaming
            var chatClient = _openAiClient.GetChatClient(_deploymentName);
            var result = await chatClient.CompleteChatAsync(messages, completionOptions);
            return result.Value.Content[0].Text;
        }

        public Task<string> SendRequest(PromptSet prompts)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompts.SystemPrompt),
                new UserChatMessage(prompts.UserPrompt)
            };

            var completionOptions = new ChatCompletionOptions()
            {
                Temperature = 0.1f,
                MaxOutputTokenCount = 500,
            };

            return SendRequest(messages, completionOptions);
        }

        public object ParseResponse(string response)
        {
            return "whoop whoop";
        }
    }
}
