using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Logging;

namespace GatheringTheMagic.Infrastructure.Services;

public class CardDrawService : ICardDrawService
{
    private readonly GameLogger _logger;

    public CardDrawService(IGameLogger logger)
    {
        //_logger = logger;
    }

    public void DrawOpeningHand(Game game, Owner owner, int count = 7)
    {
        for (int i = 0; i < count; i++)
            DrawCard(game, owner);
    }

    public CardInstance DrawCard(Game game, Owner owner)
    {
        // 1) Identify deck & hand
        var deck = owner == Owner.Player ? game.PlayerDeck : game.OpponentDeck;
        var hand = owner == Owner.Player ? game.PlayerHand : game.OpponentHand;

        // 2) Attempt draw
        CardInstance card = null;
        if (deck.Cards.Count > 0)
        {
            card = deck.Draw();
            hand.Add(card);
            //_logger.Log($"{owner} draws {(card.Definition?.Name ?? "a card")}.");
        }
        else
        {
            //_logger.Log($"{owner} attempted to draw but library was empty.");
        }

        return card;
    }
}
