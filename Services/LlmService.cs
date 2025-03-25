using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using sts_ai_support.Models;
using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public class LlmService : ILlmService
    {
        private const float TEMPERATURE = 0.5f;
        private const int MAX_TOKEN_OUTPUT_COUNT = 8000;

        private AzureKeyCredential _apiKey;
        private string _deploymentName;
        private Uri _endpoint;

        private AzureOpenAIClient _openAiClient;
        private ChatCompletionOptions _chatCompletionOptions;

        public LlmService(IOptions<AzureOpenAISettings> azureOpenAiSettings)
        {
            _apiKey = new AzureKeyCredential(azureOpenAiSettings.Value.ApiKey);
            _deploymentName = azureOpenAiSettings.Value.DeploymentName;
            _endpoint = new Uri(azureOpenAiSettings.Value.Endpoint);

            _openAiClient = new(_endpoint, _apiKey);
            _chatCompletionOptions = new ChatCompletionOptions()
            {
                Temperature = TEMPERATURE,
                MaxOutputTokenCount = MAX_TOKEN_OUTPUT_COUNT,
            };
        }

        public async Task<string> SendRequest(IEnumerable<ChatMessage> messages, ChatCompletionOptions completionOptions)
        {
            // Completion without streaming
            var chatClient = _openAiClient.GetChatClient(_deploymentName);
            var result = await chatClient.CompleteChatAsync(messages, completionOptions);
            return result.Value.Content[0].Text;
        }

        public Task<string> SendInitialRequest(PromptSet prompts)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompts.SystemPrompt),
                new UserChatMessage(prompts.UserPrompt)
            };

            return SendRequest(messages, _chatCompletionOptions);
        }

        public Task<string> ApplyChainedReasoning(PromptSet prompts)
        {
            if (prompts.Response is null)
            {
                throw new InvalidOperationException("Cannot used chain reasoning without an initial response");
            }

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompts.SystemPrompt),
                new UserChatMessage(prompts.UserPrompt),
                new AssistantChatMessage(prompts.Response),
                new UserChatMessage(prompts.ChainedPrompt)
            };
            
            return SendRequest(messages, _chatCompletionOptions);
        }

        public T ParseJsonResponse<T>(string response)
        {
            response = response.Replace("```json", "").Replace("```", "").Trim();

            var model = JsonSerializer.Deserialize<T>(response);
            return model is null
                ? throw new InvalidOperationException("Could not parse LLM response.")
                : model;
        }

        public string ParseHtmlResponse(string response)
        {
            response = response.Replace("```html", "").Replace("```", "").Trim();

            return response;
        }
    }
}
