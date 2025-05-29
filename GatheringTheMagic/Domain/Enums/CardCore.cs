namespace GatheringTheMagic.Domain.Enums
{
    /// <summary>
    /// A card’s color identity – multicolor cards simply combine multiple flags.
    /// </summary>
    [Flags]
    public enum CardColor
    {
        None = 0,
        White = 1 << 0,
        Blue = 1 << 1,
        Black = 1 << 2,
        Red = 1 << 3,
        Green = 1 << 4,
        Colorless = 1 << 5
    }

    /// <summary>
    /// The “main” types a card can have.  A card like an Artifact Creature is both flags.
    /// </summary>
    [Flags]
    public enum CardType
    {
        None = 0,
        Creature = 1 << 0,
        Artifact = 1 << 1,
        Enchantment = 1 << 2,
        Land = 1 << 3,
        Planeswalker = 1 << 4,
        Instant = 1 << 5,
        Sorcery = 1 << 6,
        Tribal = 1 << 7
    }

    /// <summary>
    /// Supertypes like Legendary or Basic.
    /// </summary>
    [Flags]
    public enum CardSupertype
    {
        None = 0,
        Basic = 1 << 0,
        Legendary = 1 << 1,
        Snow = 1 << 2,
        World = 1 << 3,
        Ongoing = 1 << 4
    }

    /// <summary>
    /// The rarity printed on the card.
    /// </summary>
    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare,
        Mythic,
        Special   // e.g. promotional or token-only
    }

    /// <summary>
    /// A selection of keyword abilities – combine them for cards that have multiple.
    /// (In practice you’d likely have a more exhaustive list or even a separate system.)
    /// </summary>
    [Flags]
    public enum KeywordAbility
    {
        None = 0,
        Flying = 1 << 0,
        Trample = 1 << 1,
        Vigilance = 1 << 2,
        Haste = 1 << 3,
        Deathtouch = 1 << 4,
        Lifelink = 1 << 5,
        FirstStrike = 1 << 6,
        Flash = 1 << 7,
        Reach = 1 << 8,
        // …etc.
    }
}
