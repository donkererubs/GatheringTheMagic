using System;
using System.Linq;
using System.Collections.Generic;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Infrastructure.Data;

namespace GatheringTheMagic.Domain.Entities;

public class Game
{
    private static readonly Random _rng = new();

    // Immutable deck‐list dictionaries...
    public IReadOnlyDictionary<CardDefinition, int> PlayerOriginalDeckList { get; }
    public IReadOnlyDictionary<CardDefinition, int> OpponentOriginalDeckList { get; }

    // Active decks
    public Deck PlayerDeck { get; private set; }
    public Deck OpponentDeck { get; private set; }


    // New: opening hands
    public List<CardInstance> PlayerHand { get; } = new();
    public List<CardInstance> OpponentHand { get; } = new();

    public List<CardInstance> PlayerBattlefield { get; } = new();
    public List<CardInstance> OpponentBattlefield { get; } = new();


    // Track how many lands each player has played this turn
    private readonly Dictionary<Owner, int> _landPlaysThisTurn = new()
    {
        { Owner.Player, 0 },
        { Owner.Opponent, 0 }
    };

    // Turn state
    public Owner ActivePlayer { get; private set; }
    public TurnPhase CurrentPhase { get; private set; }

    public Game()
    {
        // Build initial state
        Reset();
    }

    /// <summary>
    /// Completely re‐initializes the game to a fresh start:
    /// new shuffled decks, cleared hands/battlefields, opening draws,
    /// and first player on the Untap step.
    /// </summary>
    public void Reset()
    {
        // 1) Build & shuffle new decks
        PlayerDeck = GenerateRandomDeck(Owner.Player);
        OpponentDeck = GenerateRandomDeck(Owner.Opponent);
        PlayerDeck.Shuffle();
        OpponentDeck.Shuffle();

        // 2) Clear all zones
        PlayerHand.Clear();
        OpponentHand.Clear();
        PlayerBattlefield.Clear();
        OpponentBattlefield.Clear();

        // 3) Reset per-turn counters
        _landPlaysThisTurn[Owner.Player] = 0;
        _landPlaysThisTurn[Owner.Opponent] = 0;

        // 4) Opening hands: each draws 7
        DrawOpeningHand(Owner.Player);
        DrawOpeningHand(Owner.Opponent);

        // 5) Set starting player and phase
        ActivePlayer = Owner.Player;
        CurrentPhase = TurnPhase.Untap;
    }

    public void DrawOpeningHand(Owner owner)
    {
        for (int i = 0; i < 7; i++)
            DrawCard(owner);

        return;
    }

    // Draw for whoever has priority
    public CardInstance DrawCard()
    {
        return DrawCard(ActivePlayer);
    }

    // Draw for a specific owner (useful for opening hands)
    public CardInstance DrawCard(Owner owner)
    {
        var deck = owner == Owner.Player ? PlayerDeck : OpponentDeck;
        var hand = owner == Owner.Player ? PlayerHand : OpponentHand;

        var card = deck.Draw();    // removes from deck, MoveTo(Zone.Hand)
        hand.Add(card);
        return card;
    }

    public void NextTurn() =>
        ActivePlayer = ActivePlayer == Owner.Player
            ? Owner.Opponent
            : Owner.Player;

    /// <summary>
    /// Plays the given card instance from its owner’s hand onto the battlefield.
    /// </summary>
    public void PlayCard(CardInstance card)
    {
        if (card.Controller == Owner.Player)
            PlayerBattlefield.Add(card);
        else
            OpponentBattlefield.Add(card);

        // remove from hand & move zone
        if (card.Controller == Owner.Player)
            PlayerHand.Remove(card);
        else
            OpponentHand.Remove(card);

        card.MoveTo(Zone.Play);
    }


    private static Deck GenerateRandomDeck(Owner owner)
    {
        var deck = new Deck(owner);
        var definitions = SampleCards.All;

        // keep adding random cards until we hit exactly 60
        while (deck.Cards.Count < Deck.MaxDeckSize)
        {
            var def = definitions[_rng.Next(definitions.Count)];
            try { deck.Add(def); }
            catch (InvalidOperationException) { /* skip duplicates or over 60 */ }
        }

        return deck;
    }

    private static IReadOnlyDictionary<CardDefinition, int> CountByDefinition(IEnumerable<CardInstance> cards)
    {
        return cards
            .GroupBy(ci => ci.Definition)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Advance from the current phase into the next one, executing its domain logic.
    /// If you just finished Cleanup, this also flips ActivePlayer and resets back to Untap.
    /// </summary>
    public void AdvancePhase()
    {
        switch (CurrentPhase)
        {
            case TurnPhase.Untap:
                UntapStep(ActivePlayer);
                CurrentPhase = TurnPhase.Upkeep;
                break;

            case TurnPhase.Upkeep:
                UpkeepStep(ActivePlayer);
                CurrentPhase = TurnPhase.Draw;
                break;

            case TurnPhase.Draw:
                DrawCard(ActivePlayer);
                CurrentPhase = TurnPhase.Main1;
                break;

            case TurnPhase.Main1:
                // nothing automatic—UI will call PlayCard etc.
                CurrentPhase = TurnPhase.Combat;
                break;

            case TurnPhase.Combat:
                // domain combat resolution could go here
                CurrentPhase = TurnPhase.Main2;
                break;

            case TurnPhase.Main2:
                CurrentPhase = TurnPhase.End;
                break;

            case TurnPhase.End:
                // end‐of‐turn triggers
                CurrentPhase = TurnPhase.Cleanup;
                break;

            case TurnPhase.Cleanup:
                CleanupStep(ActivePlayer);
                // rotate active player
                ActivePlayer = ActivePlayer == Owner.Player
                               ? Owner.Opponent
                               : Owner.Player;
                // reset to Untap of the next player
                CurrentPhase = TurnPhase.Untap;
                break;
        }
    }

    /// <summary>
    /// Untap step: untap every permanent the given player controls,
    /// and remove summoning sickness so creatures can attack/tap next turn.
    /// </summary>
    public void UntapStep(Owner owner)
    {
        // Reset per‐turn land plays, if you do that here:
        _landPlaysThisTurn[owner] = 0;

        // Choose the battlefield for this player
        var battlefield = owner == Owner.Player
            ? PlayerBattlefield
            : OpponentBattlefield;

        // Untap and remove summoning sickness from all permanents
        foreach (var card in battlefield)
        {
            // Clear the Tapped flag (puts it untapped)
            card.Status &= ~CardStatus.Tapped;

            // Clear the SummoningSickness flag
            card.Status &= ~CardStatus.SummoningSickness;
        }
    }

    /// <summary>Can the given player play a land right now?</summary>
    public bool CanPlayLand(Owner owner) => _landPlaysThisTurn[owner] < 1;

    /// <summary>Record that the given player has just played a land.</summary>
    public void RegisterLandPlay(Owner owner) => _landPlaysThisTurn[owner]++;

    /// <summary>
    /// Upkeep step: placeholder for any upkeep triggers or effects.
    /// </summary>
    public void UpkeepStep(Owner owner)
    {
        // e.g. process “At the beginning of your upkeep” triggers
        // (left empty until you add actual triggers to your engine)
    }

    /// <summary>
    /// Cleanup step: remove damage from creatures and handle “until end of turn” cleanup.
    /// </summary>
    public void CleanupStep(Owner owner)
    {
        var battlefield = owner == Owner.Player
            ? PlayerBattlefield
            : OpponentBattlefield;

        foreach (var card in battlefield)
        {
            // remove all marked damage
            card.DamageMarked = 0;
            // if you had any “until end of turn” statuses, clear them here
            // e.g.: card.Status &= ~CardStatus.SomeTemporaryStatus;
        }
    }


}
