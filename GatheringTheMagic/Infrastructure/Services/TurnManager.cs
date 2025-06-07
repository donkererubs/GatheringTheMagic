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
        game.ActivePlayer = game.ActivePlayer == Owner.Player
            ? Owner.Opponent
            : Owner.Player;
    }

    public void AdvancePhase(Game game)
    {
        // 1) Determine next phase (wrap Cleanup → Untap)
        var next = game.CurrentPhase == TurnPhase.Cleanup
            ? TurnPhase.Untap
            : game.CurrentPhase + 1;

        // 2) Execute the handler if one exists
        if (_handlers.TryGetValue(next, out var handler))
        {
            handler.Execute(game);
        }
        else
        {
            // No automatic logic for Main1, Combat, Main2, End
            // We still advance phase below.
        }

        // 3) If wrapping from Cleanup, flip active player
        if (next == TurnPhase.Untap && game.CurrentPhase == TurnPhase.Cleanup)
        {
            game.ActivePlayer = game.ActivePlayer == Owner.Player
                ? Owner.Opponent
                : Owner.Player;
        }

        // 4) Finally, update the current phase
        game.CurrentPhase = next;
    }
}
