using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Entities;

namespace GatheringTheMagic.Domain.Interfaces;

public interface IPhaseHandler
{
    TurnPhase HandlesPhase { get; }

    void Execute(Game game);
}
