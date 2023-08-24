using GuidanceExplorer;
using GuidanceExplorer.Models;
using GuidanceExplorer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSemanticKernel();

var app = builder.Build();

app.MapPost("/api/chat",
    async (ConversationForm form, ILanguageService service) =>
        await service.GetResponseAsync(form.Prompt, form.History));

app.Run();