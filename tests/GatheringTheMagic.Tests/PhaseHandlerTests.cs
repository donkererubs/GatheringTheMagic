using System;
using System.Collections.Generic;
using System.Linq;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;
using GatheringTheMagic.Infrastructure.Logging;
using GatheringTheMagic.Infrastructure.Services;
using Xunit;

namespace GatheringTheMagic.Tests;

public class PhaseHandlerTests
{
    private Game CreateGame()
    {
        // 1. Logger & land tracker
        var logger = new TestLogger();
        var landTracker = new LandPlayTracker();

        // 2. Deck builder & draw service
        var deckBuilder = new DefaultDeckBuilder(
            new FisherYatesShuffleService(),
            SampleCards.All
        );
        var drawService = new CardDrawService(logger);

        // 3. All phase handlers
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

        // 4. Turn manager & play service
        var turnManager = new TurnManager(handlers);
        var playService = new CardPlayService(logger);

        // 5. Construct Game (calls Reset internally)
        return new Game(
            logger,
            deckBuilder,
            drawService,
            turnManager,
            playService,
            landTracker
        );
    }

    private class TestLogger : IGameLogger
    {
        public List<string> Messages { get; } = new();
        public void Log(string message) => Messages.Add(message);
    }

    [Fact]
    public void UntapPhaseHandler_ResetsTappedStatus_SummoningSickness_AndLandTracker()
    {
        // Arrange
        var game = CreateGame();
        var landTracker = new LandPlayTracker();
        var handler = new UntapPhaseHandler(landTracker);

        // Put two lands played already this turn:
        landTracker.RegisterLandPlay(Owner.Player);
        Assert.False(landTracker.CanPlayLand(Owner.Player));

        // Simulate battlefield with a tapped creature with summoning sickness
        var creatureDef = SampleCards.All.First(cd => cd.Types.HasFlag(CardType.Creature));
        var instance = new CardInstance(creatureDef, Owner.Player);
        instance.Status |= CardStatus.Tapped | CardStatus.SummoningSickness;
        game.PlayerBattlefield.Add(instance);

        // ActivePlayer must be Player
        game.ActivePlayer = Owner.Player;

        // Act
        handler.Execute(game);

        // Assert: landTracker reset
        Assert.True(landTracker.CanPlayLand(Owner.Player));

        // Assert: status flags cleared
        Assert.False((instance.Status & CardStatus.Tapped) != 0);
        Assert.False((instance.Status & CardStatus.SummoningSickness) != 0);
    }

    [Fact]
    public void DrawPhaseHandler_DrawsOneCardFromDeck()
    {
        // Arrange
        var game = CreateGame();
        var handler = new DrawPhaseHandler();
        var logger = new TestLogger();

        // Clear auto-drawn opening hand (7 cards) so deck has known size
        game.PlayerHand.Clear();
        // Remove all but 2 cards from deck
        while (game.PlayerDeck.Cards.Count > 2)
        {
            var def = game.PlayerDeck.Cards[0].Definition;
            //game.PlayerDeck.Remove(def);
        }
        Assert.Equal(2, game.PlayerDeck.Cards.Count);
        Assert.Empty(game.PlayerHand);

        // Replace drawService's logger so we capture draw log
        // (game.DrawCard internally uses the original CardDrawService instance
        // we constructed with its logger—so ensure that logger is our TestLogger)

        // Act
        handler.Execute(game);

        // Assert one card moved
        Assert.Equal(1, game.PlayerHand.Count);
        Assert.Equal(1, game.PlayerDeck.Cards.Count);

        // Because game.DrawCard logs via the CardDrawService's logger,
        // and that logger was the original TestLogger, we should see a log
        //Assert.Contains(
        //    logger.Messages,
        //    msg => msg.StartsWith("Player draws"));
    }
}
