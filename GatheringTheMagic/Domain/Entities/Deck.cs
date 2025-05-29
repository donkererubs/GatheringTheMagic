using System;
using System.Collections.Generic;
using System.Linq;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Entities;

public class Deck
{
    public const int MaxDeckSize = 60;
    public const int MaxCopiesPerCard = 4;

    public Owner Owner { get; }
    private readonly List<CardInstance> _cards = new();
    public IReadOnlyList<CardInstance> Cards => _cards;

    public Deck(Owner owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Adds up to <paramref name="quantity"/> new instances of <paramref name="definition"/>
    /// to the deck.  While the deck has fewer than 60 cards, this will enforce:
    ///  1) total ≤ 60, and 
    ///  2) ≤ 4 copies per non‐basic‐land.
    /// After 60 cards, all limits are lifted.
    /// </summary>
    public void Add(CardDefinition definition, int quantity = 1)
    {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Must add at least one card.");

        bool isBuildingPhase = _cards.Count < MaxDeckSize;

        if (isBuildingPhase)
        {
            // 1) Enforce total deck size ≤ 60
            if (_cards.Count + quantity > MaxDeckSize)
                throw new InvalidOperationException(
                    $"Cannot add {quantity} cards: deck would exceed {MaxDeckSize} cards.");

            // 2) Enforce max 4 copies (unless it's a basic land)
            if (!IsBasicLand(definition))
            {
                int existing = _cards.Count(ci => ci.Definition == definition);
                if (existing + quantity > MaxCopiesPerCard)
                    throw new InvalidOperationException(
                        $"Cannot have more than {MaxCopiesPerCard} copies of '{definition.Name}' " +
                        $"(would be {existing + quantity}).");
            }
        }

        // Once you've reached 60, or if you're simply adding after the game has started,
        // no further limits apply:
        for (int i = 0; i < quantity; i++)
            _cards.Add(new CardInstance(definition, Owner));
    }

    /// <summary>
    /// Removes up to <paramref name="quantity"/> copies of <paramref name="definition"/> from the deck.
    /// </summary>
    public void Remove(CardDefinition definition, int quantity = 1)
    {
        if (quantity < 1) return;

        var toRemove = _cards
            .Where(ci => ci.Definition == definition)
            .Take(quantity)
            .ToList();

        foreach (var inst in toRemove)
            _cards.Remove(inst);
    }

    private static bool IsBasicLand(CardDefinition definition)
    {
        // Basic lands are the only cards with Supertypes.Basic & CardType.Land
        return definition.Supertypes.HasFlag(CardSupertype.Basic)
            && definition.Types.HasFlag(CardType.Land);
    }

    /// <summary>
    /// Randomly permutes the order of the cards in this deck.
    /// </summary>
    public void Shuffle()
    {
        var rng = new Random();
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var tmp = _cards[k];
            _cards[k] = _cards[n];
            _cards[n] = tmp;
        }
    }

    /// <summary>
    /// Draws the top card from this deck (removes it from the deck list),
    /// moves it to the Hand zone, and returns the instance.
    /// </summary>
    public CardInstance Draw()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("Cannot draw from an empty deck.");

        // Take the “top” card (index 0)
        var top = _cards[0];
        _cards.RemoveAt(0);

        // Move it to hand
        top.MoveTo(Zone.Hand);

        return top;
    }
}
