using OpenAI.Chat;
using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public interface ILlmService
    {
        public Task<string> SendRequest(IEnumerable<ChatMessage> messages, ChatCompletionOptions completionOptions);
        public Task<string> SendRequest(PromptSet prompts);
        public T ParseJsonResponse<T>(string response);
        public string ParseHtmlResponse(string response);
    }
}