using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Web.Helpers;

public static class ApiMapper
{
    public static object MapState(Game game)
    {
        return new
        {
            activePlayer = game.ActivePlayer,
            currentPhase = game.CurrentPhase,
            playerDeck = game.PlayerDeck.Cards.Count,
            opponentDeck = game.OpponentDeck.Cards.Count,
            playerHand = MapCards(game.PlayerHand),
            opponentHand = MapCards(game.OpponentHand),
            playerBattlefield = MapCards(game.PlayerBattlefield),
            opponentBattlefield = MapCards(game.OpponentBattlefield)
        };
    }

    // Return IEnumerable<object> instead of List<object>
    public static IEnumerable<object> MapCards(List<CardInstance> cards) =>
        cards.Select(ci => new
        {
            name = ci.Definition.Name,
            types = Enum.GetValues<CardType>()
                         .Where(t => t != CardType.None && ci.Definition.Types.HasFlag(t))
                         .Select(t => t.ToString())
                         .ToArray(),
            statuses = Enum.GetValues<CardStatus>()
                           .Where(s => s != CardStatus.None && ci.Status.HasFlag(s))
                           .Select(s => s.ToString())
                           .ToArray()
        });
}
