using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Domain.Entities;

public class Game
{
    private readonly IGameLogger _logger;
    private readonly IDeckBuilder _deckBuilder;
    //private readonly IDeckFactory _deckFactory;
    private readonly ICardDrawService _drawService;
    private readonly ITurnManager _turnManager;
    private readonly ICardPlayService _playService;
    private readonly ILandPlayTracker _landPlayTracker;


    private static readonly Random _rng = new();
    public IReadOnlyDictionary<CardDefinition, int> PlayerOriginalDeckList { get; }
    public IReadOnlyDictionary<CardDefinition, int> OpponentOriginalDeckList { get; }

    public IDeck PlayerDeck { get; private set; }
    public IDeck OpponentDeck { get; private set; }

    public List<CardInstance> PlayerHand { get; } = new();
    public List<CardInstance> OpponentHand { get; } = new();

    public List<CardInstance> PlayerBattlefield { get; } = new();
    public List<CardInstance> OpponentBattlefield { get; } = new();

    public List<CardInstance> PlayerGraveyard { get; } = new();
    public List<CardInstance> OpponentGraveyard { get; } = new();

    // Turn state
    public Owner ActivePlayer { get; set; }
    public TurnPhase CurrentPhase { get; set; }

    public Game(
        IGameLogger logger,
        //IDeckFactory deckFactory,
        IDeckBuilder deckBuilder,
        ICardDrawService drawService,
        ITurnManager turnManager,
        ICardPlayService playService,
        ILandPlayTracker landPlayTracker)
    {
        _logger = logger;
        //_deckFactory = deckFactory;
        _deckBuilder = deckBuilder;
        _drawService = drawService;
        _turnManager = turnManager;
        _playService = playService;
        _landPlayTracker = landPlayTracker;

        Reset();
    }

    public void Reset()
    {
        // 1) Build & shuffle new decks via the factory
        //PlayerDeck = _deckFactory.CreateRandomDeck(Owner.Player);
        //OpponentDeck = _deckFactory.CreateRandomDeck(Owner.Opponent);
        PlayerDeck = _deckBuilder.BuildDeck(Owner.Player);
        OpponentDeck = _deckBuilder.BuildDeck(Owner.Opponent);

        // 2) Clear all zones
        ClearAllZones();

        // 3) Reset per-turn counters
        _landPlayTracker.Reset(Owner.Player);
        _landPlayTracker.Reset(Owner.Opponent);

        // 4) Opening hands: each draws 7
        _drawService.DrawOpeningHand(this, Owner.Player);
        _drawService.DrawOpeningHand(this, Owner.Opponent);

        // 5) Set starting player and phase
        ActivePlayer = Owner.Player;
        CurrentPhase = TurnPhase.Untap;

        _logger.Log("(Re)started game!");
    }

    public void ClearAllZones()
    {
        PlayerHand.Clear();
        OpponentHand.Clear();
        PlayerBattlefield.Clear();
        OpponentBattlefield.Clear();
        PlayerGraveyard.Clear();
        OpponentGraveyard.Clear();
    }

    public CardInstance DrawCard() => _drawService.DrawCard(this, ActivePlayer);
        
    public void PlayCard(CardInstance card) => _playService.PlayCard(this, card);

    public bool CanPlayLand() => CanPlayLand(ActivePlayer);
    public bool CanPlayLand(Owner owner) => _landPlayTracker.CanPlayLand(owner);
    public void RegisterLandPlay(Owner owner) => _landPlayTracker.RegisterLandPlay(owner);

    public void NextTurn() => _turnManager.NextTurn(this);



    /// <summary>
    /// Untap step: untap every permanent the given player controls,
    /// and remove summoning sickness so creatures can attack/tap next turn.
    /// </summary>
    public void UntapStep(Owner owner)
    {
        // Reset this player’s land‐play count
        _landPlayTracker.Reset(owner);

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
