using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GatheringTheMagic.Application.Services;

namespace GatheringTheMagic.Infrastructure.RealTime;

public class InMemoryPrivateChatHistoryService : IPrivateChatHistoryService
{
    // key is an unordered pair “alice|bob”
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _histories
        = new(StringComparer.OrdinalIgnoreCase);

    private static string Key(string u1, string u2)
    {
        var pair = new[] { u1, u2 };
        Array.Sort(pair, StringComparer.OrdinalIgnoreCase);
        return string.Join("|", pair);
    }

    public void AddMessage(string user1, string user2, ChatMessage message)
    {
        var key = Key(user1, user2);
        var list = _histories.GetOrAdd(key, _ => new List<ChatMessage>());
        lock (list)
        {
            list.Add(message);
        }
    }

    public IReadOnlyList<ChatMessage> GetHistory(string user1, string user2)
    {
        var key = Key(user1, user2);
        if (_histories.TryGetValue(key, out var list))
        {
            lock (list)
            {
                return list.OrderBy(m => m.Timestamp).ToList();
            }
        }
        return Array.Empty<ChatMessage>();
    }

    public IReadOnlyList<string> GetChatPartners(string user)
    {
        var prefix = user + "|";
        var suffix = "|" + user;
        return _histories.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                     || k.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            .Select(k =>
            {
                var parts = k.Split('|');
                return !parts[0].Equals(user, StringComparison.OrdinalIgnoreCase)
                    ? parts[0] : parts[1];
            })
            .ToList();
    }
}
