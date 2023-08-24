using System.Collections.Immutable;
using GuidanceExplorer.Indexer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.SemanticKernel;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", optional: true)
    .Build();

var kernel = Kernel.Builder
    .WithQdrantMemoryStore(configuration["Memory:Endpoint"], Int32.Parse(configuration["Memory:VectorSize"]))
    .WithAzureChatCompletionService(configuration["LanguageModel:ChatDeploymentName"],
        configuration["LanguageModel:Endpoint"], configuration["LanguageModel:ApiKey"])
    .WithAzureTextEmbeddingGenerationService(configuration["LanguageModel:EmbeddingDeploymentName"],
        configuration["LanguageModel:Endpoint"], configuration["LanguageModel:ApiKey"])
    .Build();

var matcher = new Matcher();

matcher.AddInclude("**/*.md");
matcher.AddExclude("**/structure.md");

var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo("Content")));

foreach (var file in results.Files)
{
    Console.WriteLine($"Indexing {file.Path}");

    using (var reader = new StreamReader(File.OpenRead(Path.Join("Content",file.Path))))
    {
        var sections = MarkdownSectionParser.Parse(reader, file.Path);

        for (int index = 0; index < sections.Count; index++)
        {
            await kernel.Memory.SaveInformationAsync("guidance", sections[index].Content, sections[index].Id);
        }
    }
}