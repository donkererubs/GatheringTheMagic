using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class CleanupPhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Cleanup;

    public void Execute(Game game)
    {
        game.CleanupStep(game.ActivePlayer);
    }
}
