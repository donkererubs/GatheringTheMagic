namespace GatheringTheMagic.Application.Services;

public interface IPrivateChatHistoryService
{
    void AddMessage(string user1, string user2, ChatMessage message);
    IReadOnlyList<ChatMessage> GetHistory(string user1, string user2);
    IReadOnlyList<string> GetChatPartners(string user);
}
