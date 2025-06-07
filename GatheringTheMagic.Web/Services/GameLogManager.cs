using System.Collections.Concurrent;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Web.Services
{
    /// <summary>
    /// A simple in‐memory log manager that collects strings in order
    /// and exposes them for retrieval. Thread‐safe for concurrent writes.
    /// </summary>
    public class GameLogManager : IGameLogger
    {
        // A thread‐safe queue preserving insertion order
        private readonly ConcurrentQueue<string> _logs = new();

        /// <summary>
        /// Append a log entry to the end of the queue, prefixed with a timestamp.
        /// </summary>
        public void Log(string message)
        {
            var timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601 UTC
            _logs.Enqueue($"[{timestamp}] {message}");
        }

        /// <summary>
        /// Return all accumulated log messages so far; leave log queue intact.
        /// </summary>
        public IEnumerable<string> GetAllLogs()
        {
            return _logs.ToArray();
        }

        /// <summary>
        /// Optionally clear the log (e.g. when starting a new match).
        /// </summary>
        public void Clear()
        {
            while (_logs.TryDequeue(out _)) { }
        }
    }
}
