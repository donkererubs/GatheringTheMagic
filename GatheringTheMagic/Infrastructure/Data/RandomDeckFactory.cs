using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Data;

public class RandomDeckFactory : IDeckFactory
{
    private readonly Random _rng = new();

    public Deck CreateRandomDeck(Owner owner)
    {
        var deck = new Deck(owner);
        var definitions = SampleCards.All;

        while (deck.Cards.Count < Deck.MaxDeckSize)
        {
            var def = definitions[_rng.Next(definitions.Count)];
            try
            {
                deck.Add(def);
            }
            catch (InvalidOperationException)
            {
                // skip illegal duplicates
            }
        }

        deck.Shuffle();
        return deck;
    }
}
