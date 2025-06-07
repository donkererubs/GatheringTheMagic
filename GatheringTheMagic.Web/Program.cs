using System.Text.Json.Serialization;
using GatheringTheMagic.Application.Services;
using GatheringTheMagic.Domain.Entities;            // for CardDefinition
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;         // for SampleCards
using GatheringTheMagic.Infrastructure.Logging;      // for GameLogManager
using GatheringTheMagic.Infrastructure.Services;     // for all service implementations
//using GatheringTheMagic.Web.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// —————— Dependency Injection ——————

// 1) Card pool
builder.Services
    .AddSingleton<IReadOnlyList<CardDefinition>>(_ => SampleCards.All);

// 2) Core domain services
//builder.Services.AddSingleton<IGameLogger, GameLogManager>();
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

// 3) Domain entry point
builder.Services.AddSingleton<Game>();

// 4) Application layer
builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

// —————— Endpoints ——————

// 1) POST /api/game → Start (or reset) a new game.
app.MapPost("/api/game", ([FromServices] IGameService svc) =>
{
    var result = svc.StartNewGame();
    return Results.Json(result);
})
.WithName("StartGame");

// 2) POST /api/game/draw → Draw one card.
app.MapPost("/api/game/draw", ([FromServices] IGameService svc) =>
{
    var result = svc.DrawCard();
    return Results.Json(result);
})
.WithName("DrawCard");

// 3) POST /api/game/play → Play a card.
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
})
.WithName("PlayCard");


// ────────────────────────────────────────────────────────────────────────────────
// 4) POST /api/game/phase/advance → Advance to the next phase.
// ────────────────────────────────────────────────────────────────────────────────
app.MapPost("/api/game/phase/advance", ([FromServices] IGameService svc) =>
{
    var result = svc.AdvancePhase();
    return Results.Json(result);
})
.WithName("AdvancePhase");

// ────────────────────────────────────────────────────────────────────────────────
// 5) POST /api/game/turn/next → End the current player's turn.
// ────────────────────────────────────────────────────────────────────────────────
app.MapPost("/api/game/turn/next", ([FromServices] IGameService svc) =>
{
    var result = svc.NextTurn();
    return Results.Json(result);
})
.WithName("NextTurn");

// ────────────────────────────────────────────────────────────────────────────────
// 6) GET /api/game/state → Get the full current game state.
// ────────────────────────────────────────────────────────────────────────────────
app.MapGet("/api/game/state", ([FromServices] IGameService svc) =>
{
    var result = svc.GetGameState();
    return Results.Json(result);
})
.WithName("GetGameState");


// Serve index.html for any other routes
app.MapFallbackToFile("index.html");

app.Run();

/// <summary>
/// DTO for deserializing the “play” request JSON.
/// </summary>
public class PlayRequest
{
    [JsonPropertyName("instanceId")]
    public Guid InstanceId { get; set; }
}
