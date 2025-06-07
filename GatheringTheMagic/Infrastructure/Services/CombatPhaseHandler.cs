using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class CombatPhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Combat;

    public void Execute(Game game)
    {
        // combat resolution would go here
    }
}
