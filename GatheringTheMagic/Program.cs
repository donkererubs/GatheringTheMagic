using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;
using GatheringTheMagic.Infrastructure.Logging; // Game.cs lives here

class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddTransient<IDeckFactory, RandomDeckFactory>();
                services.AddTransient<IGameLogger, GameLogger>();
                services.AddTransient<Game>();
            })
            .Build();

        // resolve and use
        var game = host.Services.GetRequiredService<Game>();
        game.Reset();
        Console.WriteLine("Game initialized!");

        // if you have background work, you can start the host:
        await host.RunAsync();
    }
}