using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Web.Helpers;
using GatheringTheMagic.Web.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Game>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();

// Start or restart the game
app.MapPost("/api/game/start", (Game game) =>
{
    game.Reset();
    return Results.Ok(ApiMapper.MapState(game));
});

// Get full state
app.MapGet("/api/game/state", (Game game) =>
    Results.Ok(ApiMapper.MapState(game))
);

// Advance phase
app.MapPost("/api/game/next-phase", (Game game) =>
{
    game.AdvancePhase();
    return Results.Ok(ApiMapper.MapState(game));
});

// Explicit draw
app.MapPost("/api/game/draw", (Game game) =>
{
    var drawn = game.DrawCard();
    return Results.Ok(new
    {
        drawn = drawn.Definition.Name,
        state = ApiMapper.MapState(game)
    });
});

// Play a card
app.MapPost("/api/game/play", (Game game, PlayRequest req) =>
{
    var owner = game.ActivePlayer;
    var hand = owner == Owner.Player ? game.PlayerHand : game.OpponentHand;

    if (req.Index < 0 || req.Index >= hand.Count)
        return Results.BadRequest("Invalid card index");

    var card = hand[req.Index];
    bool isLand = card.Definition.Types.HasFlag(CardType.Land);

    if (isLand && !game.CanPlayLand(owner) && !req.ForceLand)
    {
        return Results.BadRequest(new
        {
            error = "land-limit",
            message = "You have already played a land. To override, set ForceLand=true."
        });
    }

    hand.RemoveAt(req.Index);
    if (isLand) game.RegisterLandPlay(owner);
    game.PlayCard(card);

    if (card.Definition.Types.HasFlag(CardType.Creature))
    {
        card.Status |= CardStatus.SummoningSickness;
        if (req.TappedOnEntry)
            card.Status |= CardStatus.Tapped;
    }

    return Results.Ok(new
    {
        played = card.Definition.Name,
        state = ApiMapper.MapState(game)
    });
});

app.Run();
