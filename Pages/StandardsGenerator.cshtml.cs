using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sts_ai_support.Services;

namespace sts_ai_support.Pages
{
    [IgnoreAntiforgeryToken]
    public class StandardsGeneratorModel: PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPromptService _promptService;
        private readonly ILlmService _llmService;

        public StandardsGeneratorModel(
            ILogger<IndexModel> logger,
            IPromptService promptService,
            ILlmService llmService
        )
        {
            _logger = logger;
            _promptService = promptService;
            _llmService = llmService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string userPrompt)
        {
            var prompts = _promptService.GetStandardsPromptSet(userPrompt);

            var response = await _llmService.SendRequest(prompts);
            var content = _llmService.ParseHtmlResponse(response);

            return Partial("_HtmlResponsePartial", content);
        }
    }
}
