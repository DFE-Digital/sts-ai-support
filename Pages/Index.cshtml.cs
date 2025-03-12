using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using sts_ai_support.Enums;
using sts_ai_support.Models;
using sts_ai_support.PromptSets;
using sts_ai_support.Services;
using sts_ai_support.ViewModels;
using System.Text;
using System.Text.Json;

namespace sts_ai_support.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IIngestionService _ingestionService;
        private readonly ITransformationService _transformationService;
        private readonly IPromptService _promptService;
        private readonly ILlmService _llmService;

        [BindProperty]
        public bool IsLoaded => _ingestionService.LoadingComplete;
        
        [BindProperty]
        public PromptSetType? SelectedPromptSetType { get; set; } = PromptSetType.General;

        public IList<SelectListItem> PromptSetTypes { get; set; }

        public IEnumerable<TopicModel>? Topics { get; set; }

        public IndexModel(
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

        public async Task<IActionResult> OnPost(Guid topicId, Guid standardId)
        {
            var standard = GetStandard(topicId, standardId);
            if (standard is null)
            {
                return new JsonResult("Standard not found");
            }

            var promptSets = _promptService.GetQuestionAnswerResponsePromptSets(standard.Title, standard.Content);

            PromptSet prompts = SelectedPromptSetType switch
            {
                PromptSetType.General => promptSets.First(ps => ps is GeneralPromptSet),
                PromptSetType.Technical => promptSets.First(ps => ps is TechnicalPromptSet),
                _ => throw new ArgumentOutOfRangeException("Not a valid prompt set type")
            };

            var response = await _llmService.SendRequest(prompts);
            var responseModel = _llmService.ParseResponse(response);

            return Partial("_AiResponsePartial", responseModel);
        }

        private StandardModel? GetStandard(Guid topicId, Guid standardId)
        {
            var standard = _ingestionService.Topics
                .FirstOrDefault(t => t.Id == topicId)?
                .Standards
                .FirstOrDefault(t => t.Id == standardId);

            return standard;
        }
    }
}
