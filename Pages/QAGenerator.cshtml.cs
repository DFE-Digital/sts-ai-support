using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpenAI.Chat;
using sts_ai_support.Enums;
using sts_ai_support.Models;
using sts_ai_support.Pages.Shared;
using sts_ai_support.PromptSets;
using sts_ai_support.Services;
using sts_ai_support.ViewModels;
using System.Text;
using System.Text.Json;

namespace sts_ai_support.Pages
{
    [IgnoreAntiforgeryToken]
    public class QAGeneratorModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IIngestionService _ingestionService;
        private readonly ITransformationService _transformationService;
        private readonly IPromptService _promptService;
        private readonly ILlmService _llmService;

        private PromptSet? _promptSet;

        [BindProperty]
        public bool IsLoaded => _ingestionService.LoadingComplete;
        
        [BindProperty]
        public PromptSetType? SelectedPromptSetType { get; set; } = PromptSetType.General;

        [BindProperty]
        public string SystemPrompt { get; set; }

        public IList<SelectListItem> PromptSetTypes { get; set; }

        public IEnumerable<TopicModel>? Topics { get; set; }

        public QAGeneratorModel(
            ILogger<IndexModel> logger,
            IIngestionService ingestionService,
            ITransformationService transformationService,
            IPromptService promptService,
            ILlmService llmService
        )
        {
            _logger = logger;
            _ingestionService = ingestionService;
            _transformationService = transformationService;
            _promptService = promptService;
            _llmService = llmService;

            PromptSetTypes = new List<SelectListItem>();

            GetOrSetPromptSet(new StandardModel("", ""));
            SystemPrompt = _promptSet!.SystemPrompt;
        }

        public async Task OnGet()
        {
            PromptSetTypes = Enum.GetValues(typeof(PromptSetType))
                .Cast<PromptSetType>()
                .Select(value => new SelectListItem
                {
                    Value = value.ToString(),
                    Text = value.ToString()
                })
                .ToList();

            while (!_ingestionService.LoadingComplete)
            {
                await Task.Delay(1000);
            }

            Topics = _ingestionService.Topics;
        }

        public IActionResult OnGetLoadStandards(Guid topicId)
        {
            var topic = _ingestionService.Topics.FirstOrDefault(t => t.Id == topicId);
            if (topic == null)
            {
                return new ContentResult();
            }

            var viewModel = new StandardsViewModel
            {
                TopicId = topicId,
                Standards = topic.Standards
            };

            return Partial("_StandardsPartial", viewModel);
        }

        public IActionResult OnGetLoadStandard(Guid topicId, Guid standardId)
        {
            var standard = GetStandard(topicId, standardId);
            if (standard is null)
            {
                return Content("");
            }

            var viewModel = new StandardViewModel
            {
                Title = standard.Title,
                Content = standard.Content
            };

            return Partial("_StandardPartial", viewModel);
        }

        public async Task<IActionResult> OnPost(Guid topicId, Guid standardId, string systemPrompt)
        {
            var standard = GetStandard(topicId, standardId);
            if (standard is null)
            {
                return Partial("_AlertPartial", "Please select a standard");
            }

            GetOrSetPromptSet(standard);
            _promptSet!.SystemPrompt = systemPrompt;

            var response = await _llmService.SendInitialRequest(_promptSet!);
            _promptSet!.Response = response;
            response = await _llmService.ApplyChainedReasoning(_promptSet!);

            var viewName = nameof(Pages_Shared__QARResponsePartial).Replace("Pages_Shared_", "");
            var responseModel = _llmService.ParseJsonResponse<QARResponseViewModel>(response);
            return Partial(viewName, responseModel);
        }

        private StandardModel? GetStandard(Guid topicId, Guid standardId)
        {
            var standard = _ingestionService.Topics
                .FirstOrDefault(t => t.Id == topicId)?
                .Standards
                .FirstOrDefault(t => t.Id == standardId);

            return standard;
        }

        private void GetOrSetPromptSet(StandardModel standard)
        {
            var promptSets = _promptService.GetQuestionAnswerResponsePromptSets(standard.Title, standard.Content);

            _promptSet = SelectedPromptSetType switch
            {
                PromptSetType.General => promptSets.First(ps => ps is GeneralPromptSet),
                PromptSetType.Technical => promptSets.First(ps => ps is TechnicalPromptSet),
                _ => throw new ArgumentOutOfRangeException("Not a valid prompt set type")
            };
        }
    }
}
