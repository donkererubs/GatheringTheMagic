using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Interfaces;

public interface IDeck
{
    Owner Owner { get; }
    IReadOnlyList<CardInstance> Cards { get; }

    void Add(CardDefinition definition, int quantity = 1);

    void Remove(CardDefinition definition, int quantity = 1);

    void Shuffle();

    CardInstance Draw();
}