using sts_ai_support.Services;

public class IngestionBackgroundService : BackgroundService
{
    private readonly IngestionService _ingestionService;

    public IngestionBackgroundService(IngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _ingestionService.Ingest();
    }
}