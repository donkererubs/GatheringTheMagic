using System.Text.Json.Serialization;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Logging;
using GatheringTheMagic.Web.Models;
using GatheringTheMagic.Web.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Register Game as a singleton so its state persists across calls
builder.Services.AddSingleton<Game>();

// Register GameLogManager (singleton) for IGameLogger
builder.Services.AddSingleton<IGameLogger, GameLogManager>();

var app = builder.Build();

// ────────────────────────────────────────────────────────────────────────────────
// 1) POST /api/game → Start (or reset) a new game. Returns { deckCount, hand }.
// ────────────────────────────────────────────────────────────────────────────────
app.MapPost("/api/game", ([FromServices] Game game) =>
{
    game.Reset();

    var deckCount = game.PlayerDeck.Cards.Count;
    var handList = game.PlayerHand.Select(ci => new
    {
        name = ci.Definition.Name,
        instanceId = ci.Id  // <-- use the correct property for your CardInstance
        // imageUrl = ci.Definition.ImageUrl
    }).ToList();

    return Results.Json(new
    {
        deckCount,
        hand = handList
    });
})
.WithName("StartGame");

// ────────────────────────────────────────────────────────────────────────────────
// 2) POST /api/game/draw → Draw one card from PlayerDeck → PlayerHand.
//    Returns { deckCount, drawnCard } or drawnCard = null if deck is empty.
// ────────────────────────────────────────────────────────────────────────────────
app.MapPost("/api/game/draw", ([FromServices] Game game) =>
{
    var cardInstance = game.DrawCard();
    if (cardInstance == null)
    {
        return Results.Json(new
        {
            deckCount = game.PlayerDeck.Cards.Count,
            drawnCard = (object?)null
        });
    }

    var drawnCard = new
    {
        name = cardInstance.Definition.Name,
        instanceId = cardInstance.Id
        // imageUrl = cardInstance.Definition.ImageUrl
    };

    return Results.Json(new
    {
        deckCount = game.PlayerDeck.Cards.Count,
        drawnCard
    });
})
.WithName("DrawCard");

// ────────────────────────────────────────────────────────────────────────────────
// 3) POST /api/game/play → Play a card from PlayerHand → PlayerBattlefield.
//    Expects JSON body { "instanceId": "<GUID>" }.
//    Returns { hand: [ … ], battlefield: [ … ] }.
// ────────────────────────────────────────────────────────────────────────────────
app.MapPost("/api/game/play", async ([FromServices] Game game, HttpRequest req) =>
{
    // 1) Deserialize JSON into PlayRequest
    var payload = await req.ReadFromJsonAsync<PlayRequest>();
    // 2) Reject if missing or Guid.Empty
    if (payload == null || payload.InstanceId == Guid.Empty)
    {
        return Results.BadRequest(new { error = "Missing or invalid 'instanceId' in request body." });
    }

    // 3) Find matching card in PlayerHand
    var cardToPlay = game.PlayerHand
        .FirstOrDefault(ci => ci.Id == payload.InstanceId);

    if (cardToPlay == null)
    {
        return Results.BadRequest(new { error = $"Card with InstanceId '{payload.InstanceId}' not found in hand." });
    }

    bool isLand = cardToPlay.Definition.Types.HasFlag(CardType.Land);
    if (isLand && !game.CanPlayLand())
    {
        return Results.BadRequest(new { error = $"Already played a land card" });
    }

    // 4) Move it onto the battlefield
    game.PlayCard(cardToPlay);

    // 5) Build updated lists
    var updatedHand = game.PlayerHand.Select(ci => new
    {
        name = ci.Definition.Name,
        instanceId = ci.Id
        // imageUrl = ci.Definition.ImageUrl
    }).ToList();

    var updatedBattlefield = game.PlayerBattlefield.Select(ci => new
    {
        name = ci.Definition.Name,
        instanceId = ci.Id
        // imageUrl = ci.Definition.ImageUrl
    }).ToList();

    return Results.Json(new
    {
        hand = updatedHand,
        battlefield = updatedBattlefield
    });
})
.WithName("PlayCard");

// Serve your index.html (and related static files) as fallback
app.MapFallbackToFile("index.html");

app.Run();

/// <summary>
/// DTO for deserializing the “play” request JSON.
/// </summary>
public class PlayRequest
{
    // Match JSON property "instanceId" exactly
    [JsonPropertyName("instanceId")]
    public Guid InstanceId { get; set; }
}
