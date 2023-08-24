using GuidanceExplorer.Models;

namespace GuidanceExplorer.Services;

public interface ILanguageService
{
    Task<ChatMessage> GetResponseAsync(string prompt, List<ChatMessage> history);
}