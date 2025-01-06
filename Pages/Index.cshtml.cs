using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAI.Chat;

namespace sts_ai_support.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string? Reply { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public void OnGet() { }

        // action method that receives prompt from the form
        public async Task<IActionResult> OnPostAsync(string prompt)
        {
            var response = await CallModel(prompt);
            Reply = response;
            return Page();
        }

        private async Task<string> CallModel(string? question)
        {
            string endpoint = _config["AzureOpenAI:Endpoint"]!;
            string apiKey = _config["AzureOpenAI:ApiKey"]!;
            string deploymentName = _config["AzureOpenAI:DeploymentName"]!;

            // Instantiate OpenAIClient for Azure Open AI.
            AzureOpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(apiKey));
            ChatClient chatClient = client.GetChatClient(deploymentName);

            // Set temperature and max token size
            var completionOptions = new ChatCompletionOptions()
            {
                Temperature = 0.5f,
                MaxOutputTokenCount = 500,
            };
            
            // Completion without streaming
            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage("You are a helpful AI assistant that also talks like a pirate."),
                    new UserChatMessage(question)
                ], completionOptions);

            var reply = completion.Content[0].Text.ToString();
            return reply;
        }
    }
}
