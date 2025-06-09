using System.Collections.ObjectModel;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Entities;

public class CardInstance
{
    public Guid Id { get; }
    public CardDefinition Definition { get; }
    public Owner OriginalOwner { get; }
    public Owner Controller { get; private set; }
    public Zone CurrentZone { get; private set; }
    public int ZoneChangeCounter { get; private set; }
    public CardStatus Status { get; set; }

    private readonly Dictionary<CounterType, int> _counters = new();
    public IReadOnlyDictionary<CounterType, int> Counters => new ReadOnlyDictionary<CounterType, int>(_counters);

    private int _damageMarked;
    public int DamageMarked
    {
        get => _damageMarked;
        set => _damageMarked = Math.Max(0, value);
    }

    private readonly List<Guid> _attachedCardIds = new();
    public IReadOnlyList<Guid> AttachedCardIds => new ReadOnlyCollection<Guid>(_attachedCardIds);

    public bool IsToken { get; }
    public bool IsCopy { get; }

    public CardInstance(
        CardDefinition definition,
        Owner originalOwner,
        bool isToken = false,
        bool isCopy = false)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        OriginalOwner = originalOwner;
        Controller = originalOwner;
        Id = Guid.NewGuid();
        CurrentZone = Zone.Deck;
        Status = CardStatus.None;
        IsToken = isToken;
        IsCopy = isCopy;
    }

    public void MoveTo(Zone newZone)
    {
        if (newZone == CurrentZone) return;
        CurrentZone = newZone;
        ZoneChangeCounter++;
    }

    public void ChangeController(Owner newController) => Controller = newController;

    public void AddCounters(CounterType type, int amount = 1)
    {
        if (amount <= 0) return;
        if (_counters.ContainsKey(type))
            _counters[type] += amount;
        else
            _counters[type] = amount;
    }

    public int RemoveCounters(CounterType type, int amount = 1)
    {
        if (!_counters.TryGetValue(type, out var have) || amount <= 0)
            return 0;

        var removed = Math.Min(have, amount);
        var remain = have - removed;

        if (remain > 0) _counters[type] = remain;
        else _counters.Remove(type);

        return removed;
    }

    public void Attach(Guid cardId)
    {
        if (!_attachedCardIds.Contains(cardId))
            _attachedCardIds.Add(cardId);
    }

    public bool Detach(Guid cardId) => _attachedCardIds.Remove(cardId);

    // Optional helpers
    public bool IsTapped => Status.HasFlag(CardStatus.Tapped);
    public void Tap() => Status |= CardStatus.Tapped;
    public void Untap() => Status &= ~CardStatus.Tapped;
    public void SetSummoningSickness() => Status |= CardStatus.SummoningSickness;
    public void ClearSummoningSickness() => Status &= ~CardStatus.SummoningSickness;
}
