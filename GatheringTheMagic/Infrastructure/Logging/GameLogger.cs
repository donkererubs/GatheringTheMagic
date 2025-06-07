using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Logging;

public class GameLogger : IGameLogger
{
    public void Log(string message)
    {
        // You could add timestamps, log levels, etc.
        Console.WriteLine($"[Game] {message}");
    }
}