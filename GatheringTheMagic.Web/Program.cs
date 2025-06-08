using System.Text.Json.Serialization;
using GatheringTheMagic.Application.Services;
using GatheringTheMagic.Domain.Entities;            // for CardDefinition
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;         // for SampleCards
using GatheringTheMagic.Infrastructure.Logging;
using GatheringTheMagic.Infrastructure.RealTime;
using GatheringTheMagic.Infrastructure.Services;     // for all service implementations
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1) Card pool
builder.Services.AddSingleton<IReadOnlyList<CardDefinition>>(_ => SampleCards.All);

// 2) Core domain services
builder.Services.AddSingleton<IGameLogger, GameLogger>();
builder.Services.AddSingleton<IShuffleService, FisherYatesShuffleService>();
builder.Services.AddSingleton<IDeckBuilder, DefaultDeckBuilder>();
builder.Services.AddSingleton<ICardDrawService, CardDrawService>();
builder.Services.AddSingleton<IPhaseHandler, UntapPhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, UpkeepPhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, DrawPhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, Main1PhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, CombatPhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, Main2PhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, EndPhaseHandler>();
builder.Services.AddSingleton<IPhaseHandler, CleanupPhaseHandler>();
builder.Services.AddSingleton<ITurnManager, TurnManager>();
builder.Services.AddSingleton<ICardPlayService, CardPlayService>();
builder.Services.AddSingleton<ILandPlayTracker, LandPlayTracker>();
builder.Services.AddSingleton<IChatHistoryService, InMemoryChatHistoryService>();

// 3) Domain entry point
builder.Services.AddSingleton<Game>();

// 4) Application layer
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IUserConnectionManager, InMemoryUserConnectionManager>();
builder.Services.AddSingleton<IPrivateChatHistoryService, InMemoryPrivateChatHistoryService>();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapPost("/api/game", ([FromServices] IGameService svc) =>
{
    var result = svc.StartNewGame();
    return Results.Json(result);
}).WithName("StartGame");

app.MapPost("/api/game/draw", ([FromServices] IGameService svc) =>
{
    var result = svc.DrawCard();
    return Results.Json(result);
}).WithName("DrawCard");

app.MapPost("/api/game/play", async ([FromServices] IGameService svc, HttpRequest req) =>
{
    var payload = await req.ReadFromJsonAsync<PlayRequest>();
    if (payload?.InstanceId == Guid.Empty)
        return Results.BadRequest(new { error = "Invalid instanceId." });

    try
    {
        var result = svc.PlayCard(payload.InstanceId);
        return Results.Json(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).WithName("PlayCard");

app.MapPost("/api/game/phase/advance", ([FromServices] IGameService svc) =>
{
    var result = svc.AdvancePhase();
    return Results.Json(result);
}).WithName("AdvancePhase");

app.MapPost("/api/game/turn/next", ([FromServices] IGameService svc) =>
{
    var result = svc.NextTurn();
    return Results.Json(result);
}).WithName("NextTurn");

app.MapGet("/api/game/state", ([FromServices] IGameService svc) =>
{
    var result = svc.GetGameState();
    return Results.Json(result);
}).WithName("GetGameState");

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapHub<GameHub>("/gameHub");

app.MapFallbackToFile("index.html");
app.Run();

public class PlayRequest
{
    [JsonPropertyName("instanceId")]
    public Guid InstanceId { get; set; }
}
