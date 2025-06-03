using GatheringTheMagic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Game>();

var app = builder.Build();

app.MapPost("/api/game", ([FromServices] Game game) =>
{
    game.Reset();

    var deckCount = game.PlayerDeck.Cards.Count;
    var handList = game.PlayerHand.Select(ci => new
    {
        name = ci.Definition.Name,
        //imageUrl = ci.Definition.ImageUrl
    });

    return Results.Json(new
    {
        deckCount,
        hand = handList
    });
})
.WithName("StartGame");

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
        //imageUrl = cardInstance.Definition.ImageUrl
    };

    return Results.Json(new
    {
        deckCount = game.PlayerDeck.Cards.Count,
        drawnCard
    });
})
.WithName("DrawCard");

app.MapFallbackToFile("index.html");
app.Run();