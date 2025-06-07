using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class DefaultDeckBuilder : IDeckBuilder
{
    private readonly IShuffleService _shuffler;
    private readonly IReadOnlyList<CardDefinition> _allDefinitions;
    private readonly Random _rng = new();

    public DefaultDeckBuilder(
        IShuffleService shuffler,
        /* here you can inject SampleCards.All or a repository */
        IReadOnlyList<CardDefinition> allDefinitions)
    {
        _shuffler = shuffler;
        _allDefinitions = allDefinitions;
    }

    public IDeck BuildDeck(Owner owner)
    {
        // 1) Fill with legal cards
        var deck = new Deck(owner);
        while (deck.Cards.Count < Deck.MaxDeckSize)
        {
            var def = _allDefinitions[_rng.Next(_allDefinitions.Count)];
            try
            {
                deck.Add(def);
            }
            catch (InvalidOperationException)
            {
                // overflow or too many copies: skip
            }
        }

        // 2) Shuffle via the pluggable algorithm
        _shuffler.Shuffle((IList<CardInstance>)deck.Cards);

        return deck;
    }
}
