namespace GatheringTheMagic.Domain.Interfaces;

public interface IShuffleService
{
    void Shuffle<T>(IList<T> list);
}
