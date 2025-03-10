using sts_ai_support.Services;

public class IngestionBackgroundService : BackgroundService
{
    private readonly IIngestionService _ingestionService;

    public IngestionBackgroundService(IIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _ingestionService.Ingest();
    }
}