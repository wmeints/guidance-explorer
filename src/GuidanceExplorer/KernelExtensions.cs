using GuidanceExplorer.Services;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace GuidanceExplorer;

public static class KernelExtensions
{
    public static void AddSemanticKernel(this IServiceCollection services)
    {
        services.AddScoped(GetKernelInstance);
        services.AddScoped<ILanguageService, LanguageService>();
    }

    private static IKernel GetKernelInstance(IServiceProvider serviceProvider)
    {
        var memoryOptions = serviceProvider.GetRequiredService<IOptions<MemoryOptions>>();
        var languageModelOptions = serviceProvider.GetRequiredService<IOptions<LanguageModelOptions>>();

        var kernel = Kernel.Builder
            .WithQdrantMemoryStore(memoryOptions.Value.Endpoint, memoryOptions.Value.VectorSize)
            .WithAzureChatCompletionService(languageModelOptions.Value.Endpoint,
                languageModelOptions.Value.ChatDeploymentName, languageModelOptions.Value.ApiKey)
            .WithAzureTextEmbeddingGenerationService(languageModelOptions.Value.Endpoint,
                languageModelOptions.Value.EmbeddingDeploymentName, languageModelOptions.Value.ApiKey)
            .Build();

        return kernel;
    }
}