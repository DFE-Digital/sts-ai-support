using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using sts_ai_support.Enums;
using sts_ai_support.PromptSets;
using sts_ai_support.Services;
using System.Text.Json;

namespace sts_ai_support.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IIngestionService _ingestionService;
        private readonly IPromptService _promptService;
        private readonly ILlmService _llmService;

        [BindProperty]
        public bool IsLoaded => _ingestionService.LoadingComplete;

        [BindProperty]
        public PromptSetType? SelectedPromptSetType { get; set; }

        public IList<SelectListItem> PromptSetTypes { get; set; }

        [BindProperty]
        public string? SelectedTopicId { get; set; }

        public IList<SelectListItem> Topics { get; set; }

        [BindProperty]
        public string? SelectedStandardId { get; set; }

        public IList<SelectListItem> Standards { get; set; }

        [BindProperty]
        public string? Reply { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IIngestionService ingestionService,
            IPromptService promptService,
            ILlmService llmService
        )
        {
            _logger = logger;
            _ingestionService = ingestionService;
            _promptService = promptService;
            _llmService = llmService;

            PromptSetTypes = new List<SelectListItem>();
            Topics = new List<SelectListItem>();
            Standards = new List<SelectListItem>();
        }

        public void OnGet()
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
                Thread.Sleep(100);
            }

            Topics = _ingestionService.Topics
                .Select(topic => new SelectListItem
                {
                    Value = topic.Id.ToString(),
                    Text = topic.Title
                })
                .ToList();
        }

        public JsonResult OnGetStandards(Guid topicId)
        {
            var topic = _ingestionService.Topics.FirstOrDefault(t => t.Id == topicId);
            if (topic == null)
            {
                return new JsonResult(new List<SelectListItem>());
            }

            var standards = topic.Standards
                .Select(standard => new SelectListItem
                {
                    Value = standard.Id.ToString(),
                    Text = standard.Title
                })
                .ToList();

            return new JsonResult(standards);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var topic = _ingestionService.Topics.FirstOrDefault(topic => topic.Id.ToString() == SelectedTopicId);
            if (topic is null)
            {
                return Page();
            }

            var standard = topic.Standards.FirstOrDefault(standard => standard.Id.ToString() == SelectedStandardId);
            if (standard is null)
            {
                return Page();
            }

            var promptSets = _promptService.GetQuestionAnswerResponsePromptSets(standard.Title, standard.Content);

            PromptSet prompts = SelectedPromptSetType == PromptSetType.General
                ? promptSets.First(ps => ps is GeneralPromptSet)
                : promptSets.First(ps => ps is TechnicalPromptSet);

            var response = await _llmService.SendRequest(prompts);
            Reply = response;
            return Page();
        }
    }
}
