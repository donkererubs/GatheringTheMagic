using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class DefaultDeckBuilder : IDeckBuilder
{
    private readonly IShuffleService _shuffler;
    private readonly IReadOnlyList<CardDefinition> _allDefinitions;
    private readonly Random _rng = new();

    public const int MaxDeckSize = 60;
    public const int MaxCopiesPerCard = 4;

    public DefaultDeckBuilder(
        IShuffleService shuffler,
        IReadOnlyList<CardDefinition> allDefinitions)
    {
        _shuffler = shuffler ?? throw new ArgumentNullException(nameof(shuffler));
        _allDefinitions = allDefinitions ?? throw new ArgumentNullException(nameof(allDefinitions));
    }

    public IDeck BuildRandomDeck(Owner owner)
    {
        var originalList = new Dictionary<CardDefinition, int>();
        int totalCards = 0;

        while (totalCards < MaxDeckSize)
        {
            // pick a random card definition
            var randomCard = _allDefinitions[_rng.Next(_allDefinitions.Count)];

            // how many of this def do we already have?
            originalList.TryGetValue(randomCard, out int have);

            // basic lands are unlimited in builder‐phase; others max 4
            bool isBasicLand =
                randomCard.Supertypes.HasFlag(CardSupertype.Basic) &&
                randomCard.Types.HasFlag(CardType.Land);

            if (isBasicLand || have < MaxCopiesPerCard)
            {
                originalList[randomCard] = have + 1;
                totalCards++;
            }
        }

        var deck = new Deck(owner, originalList);

        if (deck.Cards is IList<CardInstance> list)
            _shuffler.Shuffle(list);
        else
            throw new InvalidOperationException("DefaultDeckBuilder requires Deck.Cards to be IList<CardInstance>.");

        return deck;
    }
}
