using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Domain.Entities
{
    public class Player
    {
        public Guid Id { get; }
        public string Name { get; }
        public IDeck Deck { get; }

        public List<CardInstance> Hand { get; } = new();
        public List<CardInstance> Battlefield { get; } = new();
        public List<CardInstance> Graveyard { get; } = new();

        // Game state
        public bool HasPriority { get; set; } = false;
        public int LifeTotal { get; private set; } = 20;
        public int ManaPool { get; private set; } = 0;
        public bool HasPlayedLandThisTurn { get; private set; } = false;

        public Player(Guid id, string name, IDeck deck)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Deck = deck ?? throw new ArgumentNullException(nameof(deck));
        }

        public void LoseLife(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            LifeTotal -= amount;
        }

        public void GainLife(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            LifeTotal += amount;
        }
    }
}
