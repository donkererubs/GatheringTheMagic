using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class UntapPhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Untap;

    private readonly ILandPlayTracker _landTracker;

    public UntapPhaseHandler(ILandPlayTracker landTracker)
    {
        _landTracker = landTracker;
    }

    public void Execute(Game game)
    {
        // Reset this player’s land‐play count
        _landTracker.Reset(game.ActivePlayer);

        // Untap and clear summoning sickness
        var battlefield = game.ActivePlayer == Owner.Player
            ? game.PlayerBattlefield
            : game.OpponentBattlefield;

        foreach (var card in battlefield)
        {
            card.Status &= ~CardStatus.Tapped;
            card.Status &= ~CardStatus.SummoningSickness;
        }
    }
}
