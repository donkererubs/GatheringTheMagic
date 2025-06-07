using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Domain.Interfaces;

public interface ILandPlayTracker
{
    bool CanPlayLand(Owner owner);

    void RegisterLandPlay(Owner owner);

    void Reset(Owner owner);
}
