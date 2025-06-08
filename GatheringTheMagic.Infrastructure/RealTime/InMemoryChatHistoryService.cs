using GatheringTheMagic.Application.Services;
using System.Collections.Concurrent;

namespace GatheringTheMagic.Infrastructure.RealTime;

public class InMemoryChatHistoryService : IChatHistoryService
{
    private readonly List<ChatMessage> _messages = new();
    private readonly object _lock = new();

    public void AddMessage(ChatMessage message)
    {
        lock (_lock) { _messages.Add(message); }
    }

    public IReadOnlyList<ChatMessage> GetHistory()
    {
        lock (_lock) { return _messages.ToList(); }
    }
}
