using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Infrastructure.Data
{
    public class SampleCards
    {
        public static readonly List<CardDefinition> All = new()
        {
            new CardDefinition(
                name:         "Black Lotus",
                manaCost:     "{0}",
                colorIdentity:CardColor.Colorless,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "{T}, Sacrifice Black Lotus: Add three mana of any one color."
            ),

            new CardDefinition(
                name:         "Lightning Bolt",
                manaCost:     "{R}",
                colorIdentity:CardColor.Red,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Uncommon,
                text:         "Lightning Bolt deals 3 damage to any target."
            ),

            new CardDefinition(
                name:         "Counterspell",
                manaCost:     "{U}{U}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Uncommon,
                text:         "Counter target spell."
            ),

            new CardDefinition(
                name:         "Serra Angel",
                manaCost:     "{3}{W}{W}",
                colorIdentity:CardColor.White,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Angel" },
                rarity:       CardRarity.Uncommon,
                text:         "Flying, vigilance",
                keywords:     KeywordAbility.Flying | KeywordAbility.Vigilance,
                power:        4,
                toughness:    4
            ),

            new CardDefinition(
                name:         "Shivan Dragon",
                manaCost:     "{4}{R}{R}",
                colorIdentity:CardColor.Red,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Dragon" },
                rarity:       CardRarity.Rare,
                text:         "{R}: Shivan Dragon gets +1/+0 until end of turn.",
                keywords:     KeywordAbility.Flying,
                power:        5,
                toughness:    5
            ),

            new CardDefinition(
                name:         "Time Walk",
                manaCost:     "{1}{U}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Sorcery,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "Take an extra turn after this one."
            ),

            new CardDefinition(
                name:         "Jace, the Mind Sculptor",
                manaCost:     "{2}{U}{U}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Planeswalker,
                supertypes:   CardSupertype.Legendary,
                subtypes:     new[] { "Jace" },
                rarity:       CardRarity.Mythic,
                text:         "+2: Look at the top card of target player's library. You may put that card on the bottom of that library.\n"
                           + "0: Draw three cards, then put two cards from your hand on top of your library in any order.\n"
                           + "−1: Return target spell to its owner's hand.\n"
                           + "−12: Exile all cards from target player's library, then that player shuffles their library.",
                loyalty:      3
            ),

            new CardDefinition(
                name:         "Llanowar Elves",
                manaCost:     "{G}",
                colorIdentity:CardColor.Green,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Elf", "Druid" },
                rarity:       CardRarity.Common,
                text:         "{T}: Add {G}.",
                keywords:     KeywordAbility.None,
                power:        1,
                toughness:    1
            ),

            new CardDefinition(
                name:         "Wrath of God",
                manaCost:     "{2}{W}{W}",
                colorIdentity:CardColor.White,
                types:        CardType.Sorcery,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "Destroy all creatures. They can’t be regenerated."
            ),

            new CardDefinition(
                name:         "Dark Ritual",
                manaCost:     "{B}",
                colorIdentity:CardColor.Black,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Common,
                text:         "Add {B}{B}{B}."
            ),
            
            // — additional real cards —
            new CardDefinition(
                name:         "Brainstorm",
                manaCost:     "{U}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Common,
                text:         "Draw three cards, then put two cards from your hand on top of your library in any order."
            ),

            new CardDefinition(
                name:         "Path to Exile",
                manaCost:     "{W}",
                colorIdentity:CardColor.White,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Uncommon,
                text:         "Exile target creature. Its controller may search their library for a basic land card, put that card onto the battlefield tapped, then shuffle."
            ),

            new CardDefinition(
                name:         "Birds of Paradise",
                manaCost:     "{G}",
                colorIdentity:CardColor.Green,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Bird" },
                rarity:       CardRarity.Rare,
                text:         "{T}: Add {W}, {U}, {B}, {R}, or {G}.",
                power:        0,
                toughness:    1
            ),

            new CardDefinition(
                name:         "Snapcaster Mage",
                manaCost:     "{1}{U}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Human", "Wizard" },
                rarity:       CardRarity.Rare,
                text:         "Flash. When Snapcaster Mage enters the battlefield, target instant or sorcery card in your graveyard gains flashback until end of turn. The flashback cost is equal to its mana cost.",
                keywords:     KeywordAbility.Flash,
                power:        2,
                toughness:    1
            ),

            new CardDefinition(
                name:         "Dark Confidant",
                manaCost:     "{1}{B}",
                colorIdentity:CardColor.Black,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Human", "Wizard" },
                rarity:       CardRarity.Mythic,
                text:         "At the beginning of your upkeep, reveal the top card of your library and put that card into your hand. You lose life equal to its mana value.",
                power:        2,
                toughness:    1
            ),

            new CardDefinition(
                name:         "Sol Ring",
                manaCost:     "{1}",
                colorIdentity:CardColor.Colorless,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Uncommon,
                text:         "{T}: Add {C}{C}."
            ),

            // — the five basic lands —
            new CardDefinition(
                name:         "Plains",
                manaCost:     string.Empty,
                colorIdentity:CardColor.White,
                types:        CardType.Land,
                supertypes:   CardSupertype.Basic,
                subtypes:     new[] { "Plains" },
                rarity:       CardRarity.Common,
                text:         "{T}: Add {W}."
            ),

            new CardDefinition(
                name:         "Island",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Blue,
                types:        CardType.Land,
                supertypes:   CardSupertype.Basic,
                subtypes:     new[] { "Island" },
                rarity:       CardRarity.Common,
                text:         "{T}: Add {U}."
            ),

            new CardDefinition(
                name:         "Swamp",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Black,
                types:        CardType.Land,
                supertypes:   CardSupertype.Basic,
                subtypes:     new[] { "Swamp" },
                rarity:       CardRarity.Common,
                text:         "{T}: Add {B}."
            ),

            new CardDefinition(
                name:         "Mountain",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Red,
                types:        CardType.Land,
                supertypes:   CardSupertype.Basic,
                subtypes:     new[] { "Mountain" },
                rarity:       CardRarity.Common,
                text:         "{T}: Add {R}."
            ),

            new CardDefinition(
                name:         "Forest",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Green,
                types:        CardType.Land,
                supertypes:   CardSupertype.Basic,
                subtypes:     new[] { "Forest" },
                rarity:       CardRarity.Common,
                text:         "{T}: Add {G}."
            ),

            // …after your existing entries…

            // — 20 more iconic MTG cards —
            new CardDefinition(
                name:         "Tarmogoyf",
                manaCost:     "{1}{G}",
                colorIdentity:CardColor.Green,
                types:        CardType.Creature,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Lhurgoyf" },
                rarity:       CardRarity.Mythic,
                text:         "Tarmogoyf's power is equal to the number of card types among cards in all graveyards and its toughness is that number plus 1."
            ),

            new CardDefinition(
                name:         "Thoughtseize",
                manaCost:     "{B}",
                colorIdentity:CardColor.Black,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "Target player reveals their hand. You choose a nonland card from it. That player discards that card. You lose 2 life."
            ),

            new CardDefinition(
                name:         "Swords to Plowshares",
                manaCost:     "{W}",
                colorIdentity:CardColor.White,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Uncommon,
                text:         "Exile target creature. Its controller gains life equal to its power."
            ),

            new CardDefinition(
                name:         "Fatal Push",
                manaCost:     "{B}",
                colorIdentity:CardColor.Black,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Common,
                text:         "Destroy target creature if it has mana value 2 or less. Revolt — If a permanent you controlled left the battlefield this turn, you may destroy that creature regardless of its cost."
            ),

            new CardDefinition(
                name:         "Demonic Tutor",
                manaCost:     "{1}{B}",
                colorIdentity:CardColor.Black,
                types:        CardType.Sorcery,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "Search your library for a card, put that card into your hand, then shuffle."
            ),

            new CardDefinition(
                name:         "Mox Pearl",
                manaCost:     "{0}",
                colorIdentity:CardColor.White,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "{T}: Add {W}."
            ),

            new CardDefinition(
                name:         "Mox Sapphire",
                manaCost:     "{0}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "{T}: Add {U}."
            ),

            new CardDefinition(
                name:         "Mox Jet",
                manaCost:     "{0}",
                colorIdentity:CardColor.Black,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "{T}: Add {B}."
            ),

            new CardDefinition(
                name:         "Mox Ruby",
                manaCost:     "{0}",
                colorIdentity:CardColor.Red,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "{T}: Add {R}."
            ),

            new CardDefinition(
                name:         "Mox Emerald",
                manaCost:     "{0}",
                colorIdentity:CardColor.Green,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "{T}: Add {G}."
            ),

            new CardDefinition(
                name:         "Umezawa's Jitte",
                manaCost:     "{2}",
                colorIdentity:CardColor.Colorless,
                types:        CardType.Artifact,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Equipment" },
                rarity:       CardRarity.Rare,
                text:         "Whenever equipped creature deals combat damage, put two charge counters on Umezawa's Jitte.\n"
                            + "Remove a charge counter from Umezawa's Jitte: Choose one —\n"
                            + "• Equipped creature gets +2/+2 until end of turn.\n"
                            + "• Target creature gets −1/−1 until end of turn.\n"
                            + "• You gain 2 life."
            ),

            new CardDefinition(
                name:         "Gaea's Cradle",
                manaCost:     "{0}",
                colorIdentity:CardColor.Green,
                types:        CardType.Land,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Forest" },
                rarity:       CardRarity.Rare,
                text:         "Tap: Add {G} for each creature you control."
            ),

            new CardDefinition(
                name:         "Polluted Delta",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Blue | CardColor.Black,
                types:        CardType.Land,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Island", "Swamp" },
                rarity:       CardRarity.Rare,
                text:         "Tap, Pay 1 life, Sacrifice Polluted Delta: Search your library for an Island or Swamp card, put it onto the battlefield, then shuffle."
            ),

            new CardDefinition(
                name:         "Verdant Catacombs",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Black | CardColor.Green,
                types:        CardType.Land,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Swamp", "Forest" },
                rarity:       CardRarity.Rare,
                text:         "Tap, Pay 1 life, Sacrifice Verdant Catacombs: Search your library for a Swamp or Forest card, put it onto the battlefield, then shuffle."
            ),

            new CardDefinition(
                name:         "Arid Mesa",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Red | CardColor.White,
                types:        CardType.Land,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Mountain", "Plains" },
                rarity:       CardRarity.Rare,
                text:         "Tap, Pay 1 life, Sacrifice Arid Mesa: Search your library for a Mountain or Plains card, put it onto the battlefield, then shuffle."
            ),

            new CardDefinition(
                name:         "Scalding Tarn",
                manaCost:     string.Empty,
                colorIdentity:CardColor.Red | CardColor.Blue,
                types:        CardType.Land,
                supertypes:   CardSupertype.None,
                subtypes:     new[] { "Mountain", "Island" },
                rarity:       CardRarity.Rare,
                text:         "Tap, Pay 1 life, Sacrifice Scalding Tarn: Search your library for a Mountain or Island card, put it onto the battlefield, then shuffle."
            ),

            new CardDefinition(
                name:         "Lightning Helix",
                manaCost:     "{R}{W}",
                colorIdentity:CardColor.Red | CardColor.White,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Uncommon,
                text:         "Lightning Helix deals 3 damage to any target and you gain 3 life."
            ),

            new CardDefinition(
                name:         "Liliana of the Veil",
                manaCost:     "{1}{B}{B}",
                colorIdentity:CardColor.Black,
                types:        CardType.Planeswalker,
                supertypes:   CardSupertype.Legendary,
                subtypes:     new[] { "Liliana" },
                rarity:       CardRarity.Mythic,
                text:         "+1: Each player discards a card.\n"
                            + "−2: Target player sacrifices a creature.\n"
                            + "−6: Separate all permanents target player controls into two piles. That player sacrifices all permanents in the pile of their choice.",
                loyalty:      3
            ),

            new CardDefinition(
                name:         "Emrakul, the Aeons Torn",
                manaCost:     "{15}",
                colorIdentity:CardColor.Colorless,
                types:        CardType.Creature,
                supertypes:   CardSupertype.Legendary,
                subtypes:     new[] { "Eldrazi" },
                rarity:       CardRarity.Mythic,
                text:         "This spell can’t be countered. When you cast this spell, take an extra turn after this one.\n"
                            + "Flying, protection from colored spells, annihilator 6.\n"
                            + "When Emrakul, the Aeons Torn is put into a graveyard from anywhere, its owner shuffles their graveyard into their library.",
                power:        15,
                toughness:    15
            ),

            new CardDefinition(
                name:         "Force of Will",
                manaCost:     "{3}{U}{U}",
                colorIdentity:CardColor.Blue,
                types:        CardType.Instant,
                supertypes:   CardSupertype.None,
                subtypes:     new string[0],
                rarity:       CardRarity.Rare,
                text:         "You may pay 1 life and exile a blue card from your hand rather than pay this spell’s mana cost. Counter target spell."
            ),


            new CardDefinition(
                name: "Elvish Mystic",
                manaCost: "{G}",
                colorIdentity: CardColor.Green,
                types: CardType.Creature,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Elf", "Druid" },
                rarity: CardRarity.Common,
                text: "{T}: Add {G}.",
                power: 1,
                toughness: 1
            ),

            new CardDefinition(
                name: "Goblin Guide",
                manaCost: "{R}",
                colorIdentity: CardColor.Red,
                types: CardType.Creature,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Goblin", "Scout" },
                rarity: CardRarity.Rare,
                text: "Haste. Whenever Goblin Guide attacks, defending player reveals the top card of their library. If it's a land card, that player puts it into their hand.",
                keywords: KeywordAbility.Haste,
                power: 2,
                toughness: 2
            ),

            new CardDefinition(
                name: "Thalia, Guardian of Thraben",
                manaCost: "{1}{W}",
                colorIdentity: CardColor.White,
                types: CardType.Creature,
                supertypes: CardSupertype.Legendary,
                subtypes: new[] { "Human", "Soldier" },
                rarity: CardRarity.Rare,
                text: "First strike. Noncreature spells cost {1} more to cast.",
                keywords: KeywordAbility.FirstStrike,
                power: 2,
                toughness: 1
            ),

            new CardDefinition(
                name: "Sensei's Divining Top",
                manaCost: "{1}",
                colorIdentity: CardColor.Colorless,
                types: CardType.Artifact,
                supertypes: CardSupertype.None,
                subtypes: new string[0],
                rarity: CardRarity.Uncommon,
                text: "{1}: Look at the top three cards of your library, then put them back in any order.\n{T}: Draw a card, then put Sensei's Divining Top on top of its owner's library."
            ),

            new CardDefinition(
                name: "Primeval Titan",
                manaCost: "{4}{G}{G}",
                colorIdentity: CardColor.Green,
                types: CardType.Creature,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Giant" },
                rarity: CardRarity.Mythic,
                text: "Trample. Whenever Primeval Titan enters the battlefield or attacks, you may search your library for up to two land cards, put them onto the battlefield tapped, then shuffle.",
                keywords: KeywordAbility.Trample,
                power: 6,
                toughness: 6
            ),

            new CardDefinition(
                name: "Mother of Runes",
                manaCost: "{W}",
                colorIdentity: CardColor.White,
                types: CardType.Creature,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Human", "Cleric" },
                rarity: CardRarity.Uncommon,
                text: "{T}: Target creature you control gains protection from the color of your choice until end of turn.",
                power: 1,
                toughness: 1
            ),

            new CardDefinition(
                name: "Aether Vial",
                manaCost: "{1}",
                colorIdentity: CardColor.Colorless,
                types: CardType.Artifact,
                supertypes: CardSupertype.None,
                subtypes: new string[0],
                rarity: CardRarity.Rare,
                text: "At the beginning of your upkeep, you may put a charge counter on Aether Vial.\n{T}: You may put a creature card with converted mana cost equal to the number of charge counters on Aether Vial from your hand onto the battlefield."
            ),

            new CardDefinition(
                name: "Ensnaring Bridge",
                manaCost: "{3}",
                colorIdentity: CardColor.Colorless,
                types: CardType.Artifact,
                supertypes: CardSupertype.None,
                subtypes: new string[0],
                rarity: CardRarity.Rare,
                text: "Creatures with power greater than the number of cards in your hand can’t attack."
            ),

            new CardDefinition(
                name: "Bloodbraid Elf",
                manaCost: "{2}{R}{G}",
                colorIdentity: CardColor.Red | CardColor.Green,
                types: CardType.Creature,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Elf", "Berserker" },
                rarity: CardRarity.Uncommon,
                text: "Haste. Cascade (When you cast this spell, exile cards from the top of your library until you exile a nonland card that costs less. You may cast it without paying its mana cost. Put the exiled cards on the bottom of your library in a random order.)",
                keywords: KeywordAbility.Haste,
                power: 3,
                toughness: 2
            ),

            new CardDefinition(
                name: "Batterskull",
                manaCost: "{5}",
                colorIdentity: CardColor.Colorless,
                types: CardType.Artifact,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Equipment" },
                rarity: CardRarity.Mythic,
                text: "Living weapon. Equipped creature gets +4/+4 and has vigilance and lifelink.\n{3}: Return Batterskull to its owner’s hand.\nEquip {5}."
            ),

            new CardDefinition(
                name: "Teferi, Hero of Dominaria",
                manaCost: "{3}{W}{U}",
                colorIdentity: CardColor.White | CardColor.Blue,
                types: CardType.Planeswalker,
                supertypes: CardSupertype.Legendary,
                subtypes: new[] { "Teferi" },
                rarity: CardRarity.Mythic,
                text: "+1: Draw a card. At the beginning of the next end step, untap two lands.\n−3: Put target nonland permanent into its owner’s library third from the top.\n−8: You get an emblem with 'Whenever you draw a card, exile target permanent an opponent controls.'",
                loyalty: 4
            ),

            new CardDefinition(
                name: "Rest in Peace",
                manaCost: "{1}{W}",
                colorIdentity: CardColor.White,
                types: CardType.Enchantment,
                supertypes: CardSupertype.None,
                subtypes: new string[0],
                rarity: CardRarity.Rare,
                text: "When Rest in Peace enters the battlefield, exile all graveyards. If a card or token would be put into a graveyard from anywhere, exile it instead."
            ),

            new CardDefinition(
                name: "Thought Vessel",
                manaCost: "{2}",
                colorIdentity: CardColor.Colorless,
                types: CardType.Artifact,
                supertypes: CardSupertype.None,
                subtypes: new string[0],
                rarity: CardRarity.Uncommon,
                text: "You have no maximum hand size.\n{T}: Add {C}."
            ),

            new CardDefinition(
                name: "Monastery Swiftspear",
                manaCost: "{R}",
                colorIdentity: CardColor.Red,
                types: CardType.Creature,
                supertypes: CardSupertype.None,
                subtypes: new[] { "Human", "Monk" },
                rarity: CardRarity.Uncommon,
                text: "Haste. Prowess (Whenever you cast a noncreature spell, this creature gets +1/+1 until end of turn.)",
                keywords: KeywordAbility.Haste,
                power: 1,
                toughness: 2
            ),

            new CardDefinition(
                name: "Cyclonic Rift",
                manaCost: "{1}{U}",
                colorIdentity: CardColor.Blue,
                types: CardType.Instant,
                supertypes: CardSupertype.None,
                subtypes: new string[0],
                rarity: CardRarity.Rare,
                text: "Return target nonland permanent you don’t control to its owner’s hand.\nOverload {6}{U} (You may cast this spell for its overload cost. If you do, change its text by replacing all instances of 'target' with 'each'.)"
            )
        };
    }
}
