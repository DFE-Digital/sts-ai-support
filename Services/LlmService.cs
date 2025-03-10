using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using sts_ai_support.PromptSets;
using Microsoft.Extensions.Options;
using sts_ai_support.Models;

namespace sts_ai_support.Services
{
    public class LlmService : ILlmService
    {
        private AzureKeyCredential _apiKey;
        private string _deploymentName;
        private Uri _endpoint;

        private AzureOpenAIClient _openAiClient;

        public LlmService(IOptions<AzureOpenAISettings> azureOpenAiSettings)
        {
            _apiKey = new AzureKeyCredential(azureOpenAiSettings.Value.ApiKey);
            _deploymentName = azureOpenAiSettings.Value.DeploymentName;
            _endpoint = new Uri(azureOpenAiSettings.Value.Endpoint);

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
