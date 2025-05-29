using System;
using System.Collections.Generic;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Entities;

/// <summary>
/// A fully mutable, in-game instance of a Magic card.
/// All static/card-definition data (name, cost, types, etc.) lives in CardDefinition.
/// </summary>
public class CardInstance
{
    /// <summary>Points back at the printed-card data.</summary>
    public CardDefinition Definition { get; }

    /// <summary>Unique per copy/tokens/etc.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Who brought this card into the game.  Never changes.
    /// </summary>
    public Owner OriginalOwner { get; }

    /// <summary>
    /// Who currently controls this card (can change due to spells/effects).
    /// </summary>
    public Owner Controller { get; private set; }

    /// <summary>What zone the card is in right now (Library, Battlefield, etc.).</summary>
    public Zone CurrentZone { get; private set; }

    /// <summary>
    /// Incremented each time the card moves zones.
    /// Helps distinguish “this object” from older/newer instances.
    /// </summary>
    public int ZoneChangeCounter { get; private set; }

    /// <summary>Flags like Tapped, Enchanted, PhasedOut, etc.</summary>
    public CardStatus Status { get; set; }

    /// <summary>
    /// All marker counters on the card (e.g. +1/+1, loyalty, poison, etc.).
    /// </summary>
    private readonly Dictionary<CounterType, int> _counters = new();
    public IReadOnlyDictionary<CounterType, int> Counters => _counters;

    private int _damageMarked;
    /// <summary>Damage currently marked on this creature.</summary>
    public int DamageMarked
    {
        get => _damageMarked;
        set => _damageMarked = Math.Max(0, value);
    }

    /// <summary>Other cards (auras, equips) attached to this one.</summary>
    private readonly List<Guid> _attachedCardIds = new();
    public IReadOnlyList<Guid> AttachedCardIds => _attachedCardIds;

    /// <summary>True if this is a token.  (Tokens disappear when they leave the battlefield.)</summary>
    public bool IsToken { get; }

    /// <summary>True if this is a copy created by a spell/effect.</summary>
    public bool IsCopy { get; set; }

    /// <summary>
    /// Create a new in-game card instance.
    /// </summary>
    public CardInstance(CardDefinition definition, Owner originalOwner, bool isToken = false, bool isCopy = false)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        OriginalOwner = originalOwner;
        Controller = originalOwner;
        CurrentZone = Zone.Deck;
        ZoneChangeCounter = 0;
        IsToken = isToken;
        IsCopy = isCopy;
    }

    /// <summary>
    /// Move the card to a new zone (Library, Hand, Battlefield, etc.).
    /// Automatically bumps the zone-change counter.
    /// </summary>
    public void MoveTo(Zone newZone)
    {
        if (newZone == CurrentZone) return;
        CurrentZone = newZone;
        ZoneChangeCounter++;
    }

    /// <summary>
    /// Change who currently controls this card.
    /// </summary>
    public void ChangeController(Owner newController)
    {
        Controller = newController;
    }

    /// <summary>
    /// Add the given number of counters of <paramref name="type"/>.
    /// </summary>
    public void AddCounters(CounterType type, int amount = 1)
    {
        if (amount <= 0) return;
        if (_counters.ContainsKey(type))
            _counters[type] += amount;
        else
            _counters[type] = amount;
    }

    /// <summary>
    /// Remove up to <paramref name="amount"/> counters of <paramref name="type"/>.
    /// Returns how many were actually removed.
    /// </summary>
    public int RemoveCounters(CounterType type, int amount = 1)
    {
        if (!_counters.TryGetValue(type, out var have) || amount <= 0)
            return 0;

        var removed = Math.Min(have, amount);
        var remain = have - removed;

        if (remain > 0)
            _counters[type] = remain;
        else
            _counters.Remove(type);

        return removed;
    }

    /// <summary>
    /// Attach another card (e.g. aura or equipment) to this one.
    /// </summary>
    public void Attach(Guid cardId)
    {
        if (!_attachedCardIds.Contains(cardId))
            _attachedCardIds.Add(cardId);
    }

    /// <summary>
    /// Detach an attached card.
    /// </summary>
    public bool Detach(Guid cardId)
    {
        return _attachedCardIds.Remove(cardId);
    }
}
