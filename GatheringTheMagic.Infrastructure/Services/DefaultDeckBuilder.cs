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
        IReadOnlyList<CardDefinition> allDefinitions)
    {
        _shuffler = shuffler ?? throw new ArgumentNullException(nameof(shuffler));
        _allDefinitions = allDefinitions ?? throw new ArgumentNullException(nameof(allDefinitions));
    }

    public IDeck BuildDeck(Owner owner)
    {
        // 1) Build the “deck list” with copy‐limits & size ≤ 60
        var originalList = new Dictionary<CardDefinition, int>();
        int totalCards = 0;

        while (totalCards < Deck.MaxDeckSize)
        {
            // pick a random card definition
            var def = _allDefinitions[_rng.Next(_allDefinitions.Count)];

            // how many of this def do we already have?
            originalList.TryGetValue(def, out int have);

            // basic lands are unlimited in builder‐phase; others max 4
            bool isBasicLand =
                def.Supertypes.HasFlag(CardSupertype.Basic) &&
                def.Types.HasFlag(CardType.Land);

            if (isBasicLand || have < Deck.MaxCopiesPerCard)
            {
                originalList[def] = have + 1;
                totalCards++;
            }
            // else: skip this draw and pick again
        }

        // 2) Create the Deck, which seeds CardInstances from originalList
        var deck = new Deck(owner, originalList);

        // 3) Shuffle via the injected algorithm (we know Cards is backed by a List<>)
        if (deck.Cards is IList<CardInstance> list)
            _shuffler.Shuffle(list);
        else
            throw new InvalidOperationException(
                "DefaultDeckBuilder requires Deck.Cards to be IList<CardInstance>.");

        return deck;
    }
}
