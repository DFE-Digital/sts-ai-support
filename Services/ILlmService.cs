using OpenAI.Chat;
using sts_ai_support.Models;
using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public interface ILlmService
    {
        public Task<string> SendRequest(IEnumerable<RequestChatMessageModel> messages);
        public Task<string> SendInitialRequest(PromptSet prompts);
        public Task<string> ApplyChainedReasoning(PromptSet prompts);
        public T ParseJsonResponse<T>(string response);
        public string ParseHtmlResponse(string response);
    }
}