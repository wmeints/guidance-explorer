namespace GuidanceExplorer.Models;

public class ConversationForm
{
    public string Prompt { get; set; }
    public List<ChatMessage> History { get; set; }
}