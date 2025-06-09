using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;
using GatheringTheMagic.Infrastructure.Services;

namespace GatheringTheMagic.Tests;

public class CardDrawServiceTests
{
    private class TestLogger : IGameLogger
    {
        public List<string> Messages { get; } = new();
        public void Log(string message) => Messages.Add(message);
    }

    private Game CreateEmptyGame(TestLogger logger)
    {
        // 1) Land tracker
        var landTracker = new LandPlayTracker();

        // 2) DeckBuilder
        var deckBuilder = new DefaultDeckBuilder(
            new FisherYatesShuffleService(),
            SampleCards.All
        );

        // 3) Draw service (under test)
        var drawService = new CardDrawService(logger);

        // 4) Phase handlers + turn manager
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

        // 5) Play service (unused here, but required by Game ctor)
        var playService = new CardPlayService(logger);

        // 6) Construct Game (calls Reset internally)
        var game = new Game(
            logger,
            deckBuilder,
            drawService,
            turnManager,
            playService,
            landTracker
        );

        // 7) Clear out all zones and deck
        game.PlayerHand.Clear();
        game.PlayerBattlefield.Clear();
        game.OpponentHand.Clear();
        game.OpponentBattlefield.Clear();
        game.PlayerGraveyard.Clear();
        game.OpponentGraveyard.Clear();
        // Remove every card from the deck
        while (game.PlayerDeck.Cards.Any())
        {
            var def = game.PlayerDeck.Cards[0].Definition;
           // game.PlayerDeck.Remove(def, 1);
        }
        while (game.OpponentDeck.Cards.Any())
        {
            var def = game.OpponentDeck.Cards[0].Definition;
           // game.OpponentDeck.Remove(def, 1);
        }

        return game;
    }

    [Fact]
    public void DrawCard_FromNonEmptyDeck_AddsToHandAndLogs()
    {
        // Arrange
        var logger = new TestLogger();
        var game = CreateEmptyGame(logger);

        // Put two known cards into the deck
        var def1 = SampleCards.All.First(cd => !cd.Types.HasFlag(CardType.Land));
        var def2 = SampleCards.All.First(cd => !cd.Types.HasFlag(CardType.Land) && cd != def1);
        //game.PlayerDeck.Add(def1);
        //game.PlayerDeck.Add(def2);

        var initialDeckCount = game.PlayerDeck.Cards.Count;
        Assert.Equal(2, initialDeckCount);
        Assert.Empty(game.PlayerHand);

        var drawService = new CardDrawService(logger);

        // Act
        var drawn = drawService.DrawCard(game, Owner.Player);

        // Assert
        Assert.NotNull(drawn);
        Assert.Equal(initialDeckCount - 1, game.PlayerDeck.Cards.Count);
        Assert.Single(game.PlayerHand);
        Assert.Contains(game.PlayerHand, ci => ci == drawn);
        Assert.Contains(
            logger.Messages,
            msg => msg == $"Player draws {drawn.Definition.Name}.");
    }

    [Fact]
    public void DrawCard_FromEmptyDeck_ReturnsNullAndLogs()
    {
        // Arrange
        var logger = new TestLogger();
        var game = CreateEmptyGame(logger);
        Assert.Empty(game.PlayerDeck.Cards);

        var drawService = new CardDrawService(logger);

        // Act
        var drawn = drawService.DrawCard(game, Owner.Player);

        // Assert
        Assert.Null(drawn);
        Assert.Empty(game.PlayerHand);
        Assert.Contains(
            logger.Messages,
            msg => msg == "Player attempted to draw but library was empty.");
    }

}
