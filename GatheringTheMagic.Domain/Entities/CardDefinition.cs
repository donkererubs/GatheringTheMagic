using System.Collections.Generic;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Entities;

public class CardDefinition
{
    public string Name { get; }
    public string ManaCost { get; }
    public CardColor ColorIdentity { get; }
    public CardType Types { get; }
    public CardSupertype Supertypes { get; }
    public IReadOnlyList<string> Subtypes { get; }
    public CardRarity Rarity { get; }
    public string Text { get; }
    public int? Power { get; }
    public int? Toughness { get; }
    public int? Loyalty { get; }
    public KeywordAbility Keywords { get; }

    public CardDefinition(
        string name,
        string manaCost,
        CardColor colorIdentity,
        CardType types,
        CardSupertype supertypes,
        IEnumerable<string> subtypes,
        CardRarity rarity,
        string text,
        KeywordAbility keywords = KeywordAbility.None,
        int? power = null,
        int? toughness = null,
        int? loyalty = null)
    {
        Name = name;
        ManaCost = manaCost;
        ColorIdentity = colorIdentity;
        Types = types;
        Supertypes = supertypes;
        Subtypes = new List<string>(subtypes);
        Rarity = rarity;
        Text = text;
        Keywords = keywords;
        Power = power;
        Toughness = toughness;
        Loyalty = loyalty;
    }
}
