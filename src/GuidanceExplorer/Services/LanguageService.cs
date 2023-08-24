using System.Text;
using GuidanceExplorer.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace GuidanceExplorer.Services;

public class LanguageService: ILanguageService
{
    private readonly IKernel _kernel;

    public LanguageService(IKernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<ChatMessage> GetResponseAsync(string prompt, List<ChatMessage> history)
    {
        var skills= _kernel.ImportSemanticSkillFromDirectory("Skills", "Chat");
        var context = new SKContext();

        var memoryInformation = await QueryMemoryAsync(prompt);
        
        context.Variables.Set("history", FormatChatHistory(history));
        context.Variables.Set("memory", memoryInformation);
        
        var output = await skills["Converse"].InvokeAsync(context);

        return new ChatMessage(ChatRole.Assistant, output.Result);
    }

    private string FormatChatHistory(List<ChatMessage> messages)
    {
        var outputBuilder = new StringBuilder();

        foreach (var message in messages)
        {
            outputBuilder.AppendLine($"{message.Role}: {message.Content}");
        }
        
        return outputBuilder.ToString();
    }

    private async Task<string> QueryMemoryAsync(string prompt)
    {
        var outputBuilder = new StringBuilder();
        var results = _kernel.Memory.SearchAsync("guidance", prompt);

        await foreach (var item in results)
        {
            var itemId = item.Metadata.Id;
        }

        return outputBuilder.ToString();
    }
}