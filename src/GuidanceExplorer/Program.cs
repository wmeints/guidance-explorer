using GuidanceExplorer;
using GuidanceExplorer.Models;
using GuidanceExplorer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSemanticKernel();
builder.Services.Configure<LanguageModelOptions>(builder.Configuration.GetSection("LanguageModel"));
builder.Services.Configure<MemoryOptions>(builder.Configuration.GetSection("Memory"));

var app = builder.Build();

app.MapPost("/api/chat",
    async (ConversationForm form, ILanguageService service) =>
        await service.GetResponseAsync(form.Prompt, form.History));

app.Run();