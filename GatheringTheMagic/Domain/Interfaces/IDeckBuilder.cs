using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Interfaces;

public interface IDeckBuilder
{
    IDeck BuildDeck(Owner owner);
}
