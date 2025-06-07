using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class TurnManager : ITurnManager
{
    public void NextTurn(Game game)
    {
        game.ActivePlayer = game.ActivePlayer == Owner.Player
            ? Owner.Opponent
            : Owner.Player;
    }

    public void AdvancePhase(Game game)
    {
        switch (game.CurrentPhase)
        {
            case TurnPhase.Untap:
                game.UntapStep(game.ActivePlayer);
                game.CurrentPhase = TurnPhase.Upkeep;
                break;
            case TurnPhase.Upkeep:
                game.UpkeepStep(game.ActivePlayer);
                game.CurrentPhase = TurnPhase.Draw;
                break;
            case TurnPhase.Draw:
                game.DrawCard();
                game.CurrentPhase = TurnPhase.Main1;
                break;
            case TurnPhase.Main1:
                game.CurrentPhase = TurnPhase.Combat;
                break;
            case TurnPhase.Combat:
                game.CurrentPhase = TurnPhase.Main2;
                break;
            case TurnPhase.Main2:
                game.CurrentPhase = TurnPhase.End;
                break;
            case TurnPhase.End:
                game.CurrentPhase = TurnPhase.Cleanup;
                break;
            case TurnPhase.Cleanup:
                game.CleanupStep(game.ActivePlayer);
                // end‐of‐turn: swap player & back to Untap
                game.ActivePlayer = game.ActivePlayer == Owner.Player
                    ? Owner.Opponent
                    : Owner.Player;
                game.CurrentPhase = TurnPhase.Untap;
                break;
            default:
                throw new InvalidOperationException(
                    $"Unknown phase: {game.CurrentPhase}");
        }
    }
}
