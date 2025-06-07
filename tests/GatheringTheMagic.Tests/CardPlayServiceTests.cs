using System;
using System.Collections.Generic;
using System.Linq;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;
using GatheringTheMagic.Infrastructure.Services;
using GatheringTheMagic.Infrastructure.Logging;
using Xunit;

public class CardPlayServiceTests
{
    // A trivial logger that just records messages
    private class TestLogger : IGameLogger
    {
        public List<string> Messages { get; } = new();
        public void Log(string message) => Messages.Add(message);
    }

    [Fact]
    public void PlayingSecondLandThrows()
    {
        // 1) Arrange all dependencies:

        var logger = new TestLogger();

        // Land‐play tracker
        var landTracker = new LandPlayTracker();

        // Deck builder (needed by Game.Reset, but we’ll clear hands)
        var deckBuilder = new DefaultDeckBuilder(
            new FisherYatesShuffleService(),
            SampleCards.All
        );

        // Draw service (also used by Reset)
        var drawService = new CardDrawService(logger);

        // Phase handlers & turn manager (not used in this test, but required)
        var handlers = new IPhaseHandler[]
        {
            new UntapPhaseHandler(landTracker),
            new UpkeepPhaseHandler(),
            new DrawPhaseHandler(),
            new Main1PhaseHandler(),
            new CombatPhaseHandler(),
            new Main2PhaseHandler(),
            new EndPhaseHandler(),
            new CleanupPhaseHandler()
        };
        var turnManager = new TurnManager(handlers);

        // The service under test
        var playService = new CardPlayService(logger);

        // 2) Construct Game (auto‐calls Reset internally)
        var game = new Game(
            logger,
            deckBuilder,
            drawService,
            turnManager,
            playService,
            landTracker
        );

        // 3) Clear out all auto‐drawn cards so we start from a clean slate
        game.PlayerHand.Clear();
        game.PlayerBattlefield.Clear();
        game.OpponentHand.Clear();
        game.OpponentBattlefield.Clear();
        game.PlayerGraveyard.Clear();
        game.OpponentGraveyard.Clear();

        // 4) Put exactly two land cards into the player’s hand:
        var landDef = SampleCards.All.First(cd => cd.Types.HasFlag(CardType.Land));
        var firstLand = new CardInstance(landDef, Owner.Player);
        var secondLand = new CardInstance(landDef, Owner.Player);
        game.PlayerHand.Add(firstLand);
        game.PlayerHand.Add(secondLand);

        // 5) Act & Assert:
        // Playing the first land should succeed
        playService.PlayCard(game, firstLand);
        Assert.Contains(firstLand, game.PlayerBattlefield);

        // Playing the second land this turn should throw:
        var ex = Assert.Throws<InvalidOperationException>(() =>
            playService.PlayCard(game, secondLand)
        );
        Assert.Equal("You may only play one land per turn.", ex.Message);
    }
}
