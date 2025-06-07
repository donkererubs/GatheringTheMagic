using GatheringTheMagic.Domain.Entities;

namespace GatheringTheMagic.Domain.Interfaces;

public interface ICardPlayService
{
    void PlayCard(Game game, CardInstance card);
}
