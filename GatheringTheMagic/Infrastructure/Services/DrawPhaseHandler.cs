using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class DrawPhaseHandler : IPhaseHandler
{
    public TurnPhase HandlesPhase => TurnPhase.Draw;

    public void Execute(Game game)
    {
        // Draw a card for the active player
        game.DrawCard();
    }
}
