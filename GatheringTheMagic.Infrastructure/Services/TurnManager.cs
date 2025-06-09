using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class TurnManager : ITurnManager
{
    private readonly Dictionary<TurnPhase, IPhaseHandler> _handlers;

    public TurnManager(IEnumerable<IPhaseHandler> handlers)
    {
        // Map each handler by the phase it handles
        _handlers = handlers.ToDictionary(h => h.HandlesPhase);
    }

    public void NextTurn(Game game)
    {
        // 1) Flip active player
        game.ActivePlayer = game.ActivePlayer == Owner.Player
            ? Owner.Opponent
            : Owner.Player;

        // 2) Reset to Untap
        game.CurrentPhase = TurnPhase.Untap;

        // 3) Run Untap logic if you have a handler
        if (_handlers.TryGetValue(TurnPhase.Untap, out var untapHandler))
            untapHandler.Execute(game);
    }

    public void AdvancePhase(Game game)
    {
        var oldPhase = game.CurrentPhase;
        var nextPhase = oldPhase == TurnPhase.Cleanup
            ? TurnPhase.Untap
            : oldPhase + 1;

        // If we’re wrapping into a new turn, flip active _before_ running Untap
        if (oldPhase == TurnPhase.Cleanup && nextPhase == TurnPhase.Untap)
        {
            game.ActivePlayer = game.ActivePlayer == Owner.Player
                ? Owner.Opponent
                : Owner.Player;
        }

        // Update the Game’s phase
        game.CurrentPhase = nextPhase;

        // Execute any phase‐specific logic
        if (_handlers.TryGetValue(nextPhase, out var handler))
            handler.Execute(game);
    }
}
