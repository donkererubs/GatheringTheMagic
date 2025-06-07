using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Interfaces;

public interface ICardDrawService
{
    void DrawOpeningHand(Game game, Owner owner, int count = 7);

    CardInstance DrawCard(Game game, Owner owner);
}
