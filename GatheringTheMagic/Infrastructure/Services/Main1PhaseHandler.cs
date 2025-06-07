using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class Main1PhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Main1;

    public void Execute(Game game)
    {
    }
}
