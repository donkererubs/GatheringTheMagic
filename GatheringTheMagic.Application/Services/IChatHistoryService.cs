namespace GatheringTheMagic.Application.Services;

public record ChatMessage(string User, string Message, DateTime Timestamp);

public interface IChatHistoryService
{
    void AddMessage(ChatMessage message);
    IReadOnlyList<ChatMessage> GetHistory();
}
