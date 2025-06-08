using System.Collections.Concurrent;
using GatheringTheMagic.Application.Services;

namespace GatheringTheMagic.Infrastructure.RealTime
{
    public class InMemoryUserConnectionManager : IUserConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _users = new();

        public void AddUser(string userName, string connectionId) => _users[userName] = connectionId;

        public void RemoveConnection(string connectionId)
        {
            var kvp = _users.FirstOrDefault(x => x.Value == connectionId);
            if (!string.IsNullOrEmpty(kvp.Key))
                _users.TryRemove(kvp.Key, out _);
        }

        public string GetUserByConnectionId(string connectionId) => _users.FirstOrDefault(x => x.Value == connectionId).Key;

        public string GetConnectionId(string userName) => _users.TryGetValue(userName, out var id) ? id : null;

        public IReadOnlyList<string> GetAllUsers() => _users.Keys.ToList();
    }
}
