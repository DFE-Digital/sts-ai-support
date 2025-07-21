using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using sts_ai_support.Models;
using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public class LlmService : ILlmService
    {
        private const float TEMPERATURE = 0.5f;
        private const int MAX_TOKEN_OUTPUT_COUNT = 4096;

        private string _apiKey;
        private string _apiVersion;

        private HttpClient _httpClient;

        public LlmService(IOptions<AzureOpenAISettings> azureOpenAiSettings)
        {
            _apiKey = azureOpenAiSettings.Value.ApiKey;
            _apiVersion = azureOpenAiSettings.Value.ApiVersion;

            _httpClient = new();
            _httpClient.BaseAddress = new Uri($"{azureOpenAiSettings.Value.Endpoint}/{azureOpenAiSettings.Value.DeploymentName}/");
        }

        public async Task<string> SendRequest(IEnumerable<RequestChatMessageModel> messages)
        {
            var model = new RequestModel
            {
                MaxTokens = MAX_TOKEN_OUTPUT_COUNT,
                Messages = messages,
                Temperature = TEMPERATURE
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"chat/completions?api-version={_apiVersion}&subscription-key={_apiKey}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseModel = JsonSerializer.Deserialize<LlmResponseModel>(responseContent);

            return responseModel.Choices.First().Message.Content;
       }

        public Task<string> SendInitialRequest(PromptSet prompts)
        {
            var messages = new List<RequestChatMessageModel>(
            [
                new RequestChatMessageModel(prompts.SystemPrompt, "system"),
                new RequestChatMessageModel(prompts.UserPrompt, "user")
            ]);

            return SendRequest(messages);
        }

        public Task<string> ApplyChainedReasoning(PromptSet prompts)
        {
            if (prompts.Response is null)
            {
                throw new InvalidOperationException("Cannot used chained reasoning without an initial response");
            }

            if (prompts.ChainedPrompt is null)
            {
                throw new InvalidOperationException("Cannot use chained reasoning without a chained prompt");
            }

            var messages = new List<RequestChatMessageModel>
            {
                new RequestChatMessageModel(prompts.SystemPrompt, "system"),
                new RequestChatMessageModel(prompts.UserPrompt, "user"),
                new RequestChatMessageModel(prompts.Response, "assistant"),
                new RequestChatMessageModel(prompts.ChainedPrompt, "user"),
            };
            
            return SendRequest(messages);
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
