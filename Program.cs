using sts_ai_support.Models;
using sts_ai_support.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configurationSection = builder.Configuration.GetSection("AzureOpenAI");
var configuration = configurationSection.Get<AzureOpenAISettings>();
Console.WriteLine($"Loaded config: {configuration?.Endpoint} - {configuration?.DeploymentName}");
builder.Services.Configure<AzureOpenAISettings>(configurationSection);

builder.Services
    .AddSingleton<ILlmService, LlmService>()
    .AddSingleton<IPromptService, PromptService>()
    .AddSingleton<ITransformationService, TransformationService>()
    .AddSingleton<IIngestionService, IngestionService>()
    .AddHostedService<IngestionBackgroundService>();

builder.Services
    .AddRazorPages()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
