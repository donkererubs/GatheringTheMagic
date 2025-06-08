using System.Collections.ObjectModel;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Domain.Entities;

public class Deck : IDeck
{
    private readonly Dictionary<CardDefinition, int> _originalList;
    private readonly List<CardInstance> _cards = new();
    private static readonly Random _rng = Random.Shared;
    
    public const int MaxDeckSize = 60;
    public const int MaxCopiesPerCard = 4;
    
    public Owner Owner { get; }
    public IReadOnlyList<CardInstance> Cards => _cards;
    public IReadOnlyDictionary<CardDefinition, int> OriginalList => new ReadOnlyDictionary<CardDefinition, int>(_originalList);

    public Deck(Owner owner, IDictionary<CardDefinition, int> originalList)
    {
        Owner = owner;
        if (originalList is null)
            throw new ArgumentNullException(nameof(originalList));

        // store copy of the “deck-list”
        _originalList = new Dictionary<CardDefinition, int>(originalList);

        // instantiate exactly that many CardInstance
        foreach (var kvp in _originalList)
        {
            for (int i = 0; i < kvp.Value; i++)
                _cards.Add(new CardInstance(kvp.Key, owner));
        }

        // shuffle into library
        Shuffle();
    }

    public void Shuffle()
    {
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
        }
    }

    public CardInstance Draw()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("Deck is empty.");
        var top = _cards[0];
        _cards.RemoveAt(0);
        return top;
    }
}
