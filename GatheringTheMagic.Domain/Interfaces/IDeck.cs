using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Interfaces;

public interface IDeck
{
    Owner Owner { get; }
    IReadOnlyDictionary<CardDefinition, int> OriginalList { get; }
    
    IReadOnlyList<CardInstance> Cards { get; }

    void Shuffle();

    CardInstance Draw();
}