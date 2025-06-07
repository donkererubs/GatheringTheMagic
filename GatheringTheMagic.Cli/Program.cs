using System;
using System.Linq;
using System.Collections.Generic;
using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Start a new game? (y/n): ");
            if (!IsYes(Console.ReadLine()))
            {
                Console.WriteLine("Goodbye!");
                return;
            }

            // 1) Initialize game and opening hands
            var game = new Game(null, null);
            for (int i = 0; i < 7; i++) game.DrawCard(Owner.Player);
            for (int i = 0; i < 7; i++) game.DrawCard(Owner.Opponent);

            Console.WriteLine("\n🚀 Game started!");
            Console.WriteLine($"Player:   {game.PlayerDeck.Cards.Count} in deck,   {game.PlayerHand.Count} in hand");
            Console.WriteLine($"Opponent: {game.OpponentDeck.Cards.Count} in deck, {game.OpponentHand.Count} in hand");

            // 2) Phase loop
            while (true)
            {
                var active = game.ActivePlayer;
                var phase = game.CurrentPhase;
                var whoText = active == Owner.Player ? "You" : "Opponent";
                var deck = active == Owner.Player ? game.PlayerDeck : game.OpponentDeck;
                var hand = active == Owner.Player ? game.PlayerHand : game.OpponentHand;
                var battlefield = active == Owner.Player ? game.PlayerBattlefield : game.OpponentBattlefield;

                Console.WriteLine($"\n=== {whoText}'s {phase} ===");

                // Phase-specific UI
                switch (phase)
                {
                    case TurnPhase.Untap:
                        Console.WriteLine($"{whoText} have these cards in play:");
                        if (battlefield.Count == 0)
                        {
                            Console.WriteLine(" - Nothing");
                        }
                        else
                        {
                            foreach (var card in battlefield)
                            {
                                var statuses = new[]
                                {
                                    card.Status.HasFlag(CardStatus.Tapped) ? "tapped" : "untapped",
                                    card.Status.HasFlag(CardStatus.SummoningSickness) ? "summoning sickness" : null
                                }
                                .Where(s => s != null);

                                var typeNames = Enum.GetValues(typeof(CardType))
                                    .Cast<CardType>()
                                    .Where(t => t != CardType.None && card.Definition.Types.HasFlag(t))
                                    .Select(t => t.ToString());
                                
                                var typeText = string.Join(" | ", typeNames);

                                Console.WriteLine($" - [{typeText}] {card.Definition.Name} ({string.Join(", ", statuses)})");
                            }
                        }
                        break;

                    case TurnPhase.Upkeep:
                        Console.WriteLine("Upkeep step. (Apply any upkeep triggers.)");
                        break;

                    case TurnPhase.Draw:
                        // Perform the draw by advancing phase
                        {
                            int preCount = hand.Count;
                            Console.Write("Ready to draw? (press Enter) ");
                            Console.ReadLine();
                            game.AdvancePhase();  // domain will draw and move to Main1
                            var drawn = hand.Last();
                            Console.WriteLine($"{whoText} drew: {drawn.Definition.Name}");
                            Console.WriteLine($"Remaining in deck: {deck.Cards.Count}");
                        }
                        // skip the "continue" prompt since we've advanced already
                        continue;

                    case TurnPhase.Main1:
                    case TurnPhase.Main2:
                        RunMainPhase(game, active, whoText, hand);
                        break;

                    case TurnPhase.Combat:
                        Console.WriteLine("Combat phase (not implemented).");
                        break;

                    case TurnPhase.End:
                        Console.WriteLine("End step (apply end-of-turn triggers).");
                        break;

                    case TurnPhase.Cleanup:
                        Console.WriteLine("Cleanup step (remove damage & end-of-turn effects).");
                        break;
                }

                // Progression prompt
                if (phase != TurnPhase.Cleanup)
                {
                    Console.Write("Continue to next phase? (y/n): ");
                    if (IsYes(Console.ReadLine()))
                    {
                        game.AdvancePhase();
                    }
                }
                else
                {
                    Console.Write("End your turn? (y/n): ");
                    if (IsYes(Console.ReadLine()))
                    {
                        game.AdvancePhase();  // cleans up and passes to next player
                    }
                }
            }
        }

        /// <summary>
        /// In a main phase, allow the active player to play any number of cards.
        /// </summary>
        private static void RunMainPhase(
            Game game,
            Owner active,
            string whoText,
            List<CardInstance> hand)
        {
            while (true)
            {
                Console.WriteLine($"\n{whoText}'s hand ({hand.Count}):");
                if (hand.Count == 0)
                {
                    Console.WriteLine("  (no cards)");
                    break;
                }

                for (int i = 0; i < hand.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {hand[i].Definition.Name}");
                }

                Console.Write($"Select a card to play (1-{hand.Count}) or 0 to finish phase: ");
                var input = Console.ReadLine()?.Trim();
                if (!int.TryParse(input, out var choice))
                {
                    Console.WriteLine("Invalid input; please enter a number.");
                    continue;
                }
                if (choice == 0)
                    break;

                if (choice < 1 || choice > hand.Count)
                {
                    Console.WriteLine($"Please choose a number between 0 and {hand.Count}.");
                    continue;
                }

                var card = hand[choice - 1];

                // Check land play limit
                bool isLand = card.Definition.Types.HasFlag(CardType.Land);
                if (isLand && !game.CanPlayLand(active))
                {
                    Console.Write("You’ve already played a land this turn. Play a second? (y/n): ");
                    if (!IsYes(Console.ReadLine()))
                        continue;
                }

                // Creature entry prompt
                bool tappedOnEntry = false;
                bool isCreature = card.Definition.Types.HasFlag(CardType.Creature);
                if (isCreature)
                {
                    Console.Write("Should this creature enter tapped? (y/n): ");
                    tappedOnEntry = IsYes(Console.ReadLine());
                }

                // Execute play
                hand.RemoveAt(choice - 1);
                if (isLand) game.RegisterLandPlay(active);
                game.PlayCard(card);

                if (isCreature)
                {
                    card.Status |= CardStatus.SummoningSickness;
                    if (tappedOnEntry)
                        card.Status |= CardStatus.Tapped;
                }

                Console.WriteLine($"{whoText} plays: {card.Definition.Name}" +
                                  (isCreature
                                      ? tappedOnEntry
                                          ? " (entered tapped with summoning sickness)"
                                          : " (entered untapped with summoning sickness)"
                                      : ""));
            }
        }

        private static bool IsYes(string input) =>
            input?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true;
    }
}
