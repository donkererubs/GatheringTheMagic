using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class EndPhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.End;

    public void Execute(Game game)
    {
        // process end‐of‐turn triggers if/when added
    }
}
