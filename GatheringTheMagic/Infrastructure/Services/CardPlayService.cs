using System.Runtime.CompilerServices;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class CardPlayService(IGameLogger logger) : ICardPlayService
{
    private readonly IGameLogger _logger = logger;

    public void PlayCard(Game game, CardInstance card)
    {
        // --- Enforce one-land-per-turn here ---
        bool isLand = card.Definition.Types.HasFlag(CardType.Land);
        if (isLand && !game.CanPlayLand(card.Controller))
            throw new InvalidOperationException("You may only play one land per turn.");

        // 1) Put on battlefield
        var battlefield = card.Controller == Owner.Player
            ? game.PlayerBattlefield
            : game.OpponentBattlefield;
        battlefield.Add(card);

        // 2) Remove from hand
        var hand = card.Controller == Owner.Player
            ? game.PlayerHand
            : game.OpponentHand;
        hand.Remove(card);

        // 3) Update zone
        card.MoveTo(Zone.Play);

        // 4) If it’s a land, let Game track that
        if (isLand)
            game.RegisterLandPlay(card.Controller);

        // 5) Log the play
        _logger.Log($"{card.Controller} plays {card.Definition?.Name ?? "a card"}.");
    }
}
