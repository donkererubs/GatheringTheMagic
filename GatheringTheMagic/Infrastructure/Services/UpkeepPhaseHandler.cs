using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class UpkeepPhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Upkeep;

    public void Execute(Game game)
    {
        // Placeholder for upkeep triggers
        game.UpkeepStep(game.ActivePlayer);
    }
}
