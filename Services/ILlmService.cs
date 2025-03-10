using OpenAI.Chat;
using sts_ai_support.PromptSets;

namespace sts_ai_support.Services
{
    public interface ILlmService
    {
        object ParseResponse(string response);
        Task<string> SendRequest(IEnumerable<ChatMessage> messages, ChatCompletionOptions completionOptions);
        Task<string> SendRequest(PromptSet prompts);
    }
}