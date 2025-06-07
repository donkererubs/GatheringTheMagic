using System;
using System.Linq;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;
using GatheringTheMagic.Infrastructure.Data;
using GatheringTheMagic.Infrastructure.Logging;
using GatheringTheMagic.Infrastructure.Services;
using Xunit;

namespace GatheringTheMagic.Tests;

public class TurnManagerTests
{
    private class TestLogger : IGameLogger
    {
        public void Log(string message) { /* no-op */ }
    }

    private Game CreateGame()
    {
        // 1) Logger & land tracker
        var logger = new TestLogger();
        var landTracker = new LandPlayTracker();

        // 2) Deck builder & draw service
        var deckBuilder = new DefaultDeckBuilder(
            new FisherYatesShuffleService(),
            SampleCards.All
        );
        var drawService = new CardDrawService(logger);

        // 3) Phase handlers
        IPhaseHandler[] handlers = new IPhaseHandler[]
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

        // 4) Turn manager & play service
        var turnManager = new TurnManager(handlers);
        var playService = new CardPlayService(logger);

        // 5) Construct Game (calls Reset internally)
        return new Game(
            logger,
            deckBuilder,
            drawService,
            turnManager,
            playService,
            landTracker
        );
    }

    [Fact]
    public void NextTurn_FlipsActivePlayer()
    {
        var game = CreateGame();
        Assert.Equal(Owner.Player, game.ActivePlayer);

        game.NextTurn();
        Assert.Equal(Owner.Opponent, game.ActivePlayer);

        game.NextTurn();
        Assert.Equal(Owner.Player, game.ActivePlayer);
    }

    [Fact]
    public void AdvancePhase_FromUntap_ToUpkeep()
    {
        var game = CreateGame();
        // After Reset, CurrentPhase is Untap
        Assert.Equal(TurnPhase.Untap, game.CurrentPhase);
        Assert.Equal(Owner.Player, game.ActivePlayer);

        game.AdvancePhase();
        Assert.Equal(TurnPhase.Upkeep, game.CurrentPhase);
        // ActivePlayer shouldn’t change until Cleanup wraps
        Assert.Equal(Owner.Player, game.ActivePlayer);
    }

    [Fact]
    public void AdvancePhase_CyclesThroughAllPhasesAndWraps()
    {
        var game = CreateGame();
        // Define the expected sequence of phases after each AdvancePhase() call
        var expectedPhases = new[]
        {
            TurnPhase.Upkeep,
            TurnPhase.Draw,
            TurnPhase.Main1,
            TurnPhase.Combat,
            TurnPhase.Main2,
            TurnPhase.End,
            TurnPhase.Cleanup,
            TurnPhase.Untap  // wrap‐around
        };

        // ActivePlayer remains Player until wrap
        var expectedOwners = new[]
        {
            Owner.Player, // Upkeep
            Owner.Player, // Draw
            Owner.Player, // Main1
            Owner.Player, // Combat
            Owner.Player, // Main2
            Owner.Player, // End
            Owner.Player, // Cleanup
            Owner.Opponent // Untap on wrap
        };

        for (int i = 0; i < expectedPhases.Length; i++)
        {
            game.AdvancePhase();
            Assert.Equal(expectedPhases[i], game.CurrentPhase);
            Assert.Equal(expectedOwners[i], game.ActivePlayer);
        }
    }
}
