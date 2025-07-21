using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sts_ai_support.Pages.Shared;
using sts_ai_support.PromptSets;
using sts_ai_support.Services;
using sts_ai_support.ViewModels;

namespace sts_ai_support.Pages
{
    [IgnoreAntiforgeryToken]
    public class StandardsGeneratorModel: PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPromptService _promptService;
        private readonly ILlmService _llmService;

        private PromptSet? _promptSet;

        [BindProperty]
        public string SystemPrompt { get; set; }

        public StandardsGeneratorModel(
            ILogger<IndexModel> logger,
            IPromptService promptService,
            ILlmService llmService
        )
        {
            _logger = logger;
            _promptService = promptService;
            _llmService = llmService;

            _promptSet = _promptService.GetStandardsPromptSet("");
            SystemPrompt = _promptSet.SystemPrompt;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string userPrompt, string systemPrompt)
        {
            _promptSet = _promptService.GetStandardsPromptSet(userPrompt);
            _promptSet.SystemPrompt = systemPrompt;

            var response = await _llmService.SendInitialRequest(_promptSet);
            _promptSet.Response = response;
            response = await _llmService.ApplyChainedReasoning(_promptSet);

            var viewName = nameof(Pages_Shared__StandardsResponsePartial).Replace("Pages_Shared_", "");
            var responseModel = _llmService.ParseJsonResponse<StandardsResponseViewModel>(response);
            return Partial(viewName, responseModel);
        }
    }
}
