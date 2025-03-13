using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using sts_ai_support.PromptSets;
using Microsoft.Extensions.Options;
using sts_ai_support.Models;
using System.Text.Json;
using sts_ai_support.ViewModels;

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
                MaxOutputTokenCount = 4000,
            };

            return SendRequest(messages, completionOptions);
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
