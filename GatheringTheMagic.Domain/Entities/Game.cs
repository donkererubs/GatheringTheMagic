using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Domain.Entities
{
    public class Game
    {
        private readonly IGameLogger _logger;
        private readonly IDeckBuilder _deckBuilder;
        private readonly ICardDrawService _drawService;
        private readonly ITurnManager _turnManager;
        private readonly ICardPlayService _playService;
        private readonly ILandPlayTracker _landPlayTracker;

        // —— Priority tracking ——
        // Who currently has priority?
        private Owner _priorityHolder;
        // Have each player passed since the last priority reset?
        private bool _playerPassed;
        private bool _opponentPassed;

        public IDeck PlayerDeck { get; private set; }
        public IDeck OpponentDeck { get; private set; }

        public List<CardInstance> PlayerHand { get; } = new();
        public List<CardInstance> OpponentHand { get; } = new();
        public List<CardInstance> PlayerBattlefield { get; } = new();
        public List<CardInstance> OpponentBattlefield { get; } = new();
        public List<CardInstance> PlayerGraveyard { get; } = new();
        public List<CardInstance> OpponentGraveyard { get; } = new();

        // —— Turn state ——
        public Owner ActivePlayer { get; set; }
        public TurnPhase CurrentPhase { get; set; }

        // —— Expose priority for the UI/Hub ——
        public Owner PriorityHolder => _priorityHolder;
        public bool IsPlayerPriority => _priorityHolder == Owner.Player;
        public bool IsOpponentPriority => _priorityHolder == Owner.Opponent;

        public Game(
            IGameLogger logger,
            IDeckBuilder deckBuilder,
            ICardDrawService drawService,
            ITurnManager turnManager,
            ICardPlayService playService,
            ILandPlayTracker landPlayTracker)
        {
            _logger = logger;
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

            // 5) Set starting player, phase, and priority
            ActivePlayer = Owner.Player;
            CurrentPhase = TurnPhase.Untap;
            _priorityHolder = ActivePlayer;
            _playerPassed = false;
            _opponentPassed = false;

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

        // —— Existing game actions ——
        public CardInstance DrawCard() => _drawService.DrawCard(this, ActivePlayer);
        public void PlayCard(CardInstance card) => _playService.PlayCard(this, card);

        public bool CanPlayLand() => CanPlayLand(ActivePlayer);
        public bool CanPlayLand(Owner owner) => _landPlayTracker.CanPlayLand(owner);
        public void RegisterLandPlay(Owner owner) => _landPlayTracker.RegisterLandPlay(owner);

        public void AdvancePhase() => _turnManager.AdvancePhase(this);
        public void NextTurn() => _turnManager.NextTurn(this);

        public void UntapStep(Owner owner)
        {
            _landPlayTracker.Reset(owner);
            var battlefield = owner == Owner.Player
                ? PlayerBattlefield
                : OpponentBattlefield;

            foreach (var c in battlefield)
            {
                c.Status &= ~CardStatus.Tapped;
                c.Status &= ~CardStatus.SummoningSickness;
            }
        }

        public void UpkeepStep(Owner owner)
        {
            // e.g. trigger upkeep abilities
        }

        public void CleanupStep(Owner owner)
        {
            var battlefield = owner == Owner.Player
                ? PlayerBattlefield
                : OpponentBattlefield;

            foreach (var c in battlefield)
            {
                c.DamageMarked = 0;
                // clear other “until end of turn” flags here
            }
        }

        // —— NEW: Priority control ——
        public void PassPriority()
        {
            // 1. Mark that the current priority-holder has passed
            if (_priorityHolder == Owner.Player) _playerPassed = true;
            else _opponentPassed = true;

            // 2. Flip priority
            _priorityHolder = _priorityHolder == Owner.Player
                ? Owner.Opponent
                : Owner.Player;

            _logger.Log($"{_priorityHolder} has priority now.");

            // 3. If both have passed in succession, end the phase (or turn)
            if (_playerPassed && _opponentPassed)
            {
                if (CurrentPhase == TurnPhase.Cleanup)
                    _turnManager.NextTurn(this);
                else
                    _turnManager.AdvancePhase(this);

                // reset pass flags
                _playerPassed = false;
                _opponentPassed = false;

                // new active gets priority at start of phase/turn
                _priorityHolder = ActivePlayer;
                _logger.Log($"--> Phase is now {CurrentPhase}, priority returns to {ActivePlayer}.");
            }
        }
    }
}
