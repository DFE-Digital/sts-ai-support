using OpenAI.Chat;
using sts_ai_support.PromptSets;
using sts_ai_support.ViewModels;

namespace sts_ai_support.Services
{
    public interface ILlmService
    {
        public LlmResponseViewModel ParseResponse(string response);
        public Task<string> SendRequest(IEnumerable<ChatMessage> messages, ChatCompletionOptions completionOptions);
        public Task<string> SendRequest(PromptSet prompts);
    }
}