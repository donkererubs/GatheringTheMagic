using GatheringTheMagic.Domain.Entities;

namespace GatheringTheMagic.Domain.Interfaces;

public interface ITurnManager
{   
    void AdvancePhase(Game game);
        
    void NextTurn(Game game);
}
