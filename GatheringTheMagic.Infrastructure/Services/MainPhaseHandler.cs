using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class Main2PhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Main2;

    public void Execute(Game game)
    {
    }
}
