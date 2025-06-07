using global::GatheringTheMagic.Domain.Entities;
using global::GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Interfaces;

public interface IDeckFactory
{
    /// <summary>
    /// Builds a shuffled 60-card deck for the given owner.
    /// </summary>
    Deck CreateRandomDeck(Owner owner);
}