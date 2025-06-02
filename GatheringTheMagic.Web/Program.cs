using System.Linq;
using GatheringTheMagic.Domain;   // Your Game, CardInstance, etc.
using GatheringTheMagic.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;     // for [FromServices]
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1) Register a single Game instance in DI so it persists between calls
builder.Services.AddSingleton<Game>();

var app = builder.Build();

// 2) POST /api/game → start the game (opening hand of 7)
//    Returns JSON: { deckCount: int, hand: [ { name, imageUrl }, … ] }
app.MapPost("/api/game", ([FromServices] Game game) =>
{
    // If you want to restart the same singleton Game each time:
    game.Reset();             // <-- assume you have a Reset() method; otherwise new Game() on startup is fine
    //game.DrawOpeningHand();   // <-- draws 7 cards into game.Hand

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

// 3) POST /api/game/draw → draw one card from the deck
//    Returns JSON: { deckCount: int, drawnCard: { name, imageUrl } }
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


//using GatheringTheMagic.Domain;   // namespace where Game lives
//using GatheringTheMagic.Domain.Entities;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;

//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//// POST /api/game: start a new game, draw opening hand, return deck count + hand list
//app.MapPost("/api/game", () =>
//{
//    var game = new Game();

//    var deckCount = game.PlayerDeck.Cards.Count;
//    var handList = game.PlayerHand.Select(ci => new
//    {
//        ci.Definition.Name//,
//        //ImageUrl = ci.Definition.   // or whatever property gives an image URL
//    });

//    return Results.Json(new
//    {
//        DeckCount = deckCount,
//        Hand = handList
//    });
//})
//.WithName("StartGame"); // naming isn’t strictly required, but can help if you generate links later :contentReference[oaicite:0]{index=0}


//app.MapPost("/api/game/draw", (Game game) =>
//{
//    // 3a) Call DrawCard(); assume it returns a CardInstance or null if deck empty
//    var cardInstance = game.DrawCard();

//    if (cardInstance == null)
//    {
//        // No more cards: return deckCount plus null drawnCard
//        return Results.Json(new
//        {
//            deckCount = game.PlayerDeck.Cards.Count,
//            drawnCard = (object?)null
//        });
//    }

//    var drawnCard = new
//    {
//        name = cardInstance.Definition.Name
//        //, imageUrl = cardInstance.Definition.ImageUrl
//    };

//    return Results.Json(new
//    {
//        deckCount = game.PlayerDeck.Cards.Count,
//        drawnCard
//    });
//})
//.WithName("DrawCard");

//app.MapFallbackToFile("index.html");
//app.Run();