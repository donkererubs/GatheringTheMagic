using GatheringTheMagic.Domain.Enums;
using GatheringTheMagic.Domain.Interfaces;

namespace GatheringTheMagic.Infrastructure.Services;

public class LandPlayTracker : ILandPlayTracker
{
    private readonly Dictionary<Owner, int> _landsThisTurn = new()
    {
        { Owner.Player,   0 },
        { Owner.Opponent, 0 }
    };

    public bool CanPlayLand(Owner owner) => _landsThisTurn[owner] < 1;

    public void RegisterLandPlay(Owner owner) => _landsThisTurn[owner]++;

    public void Reset(Owner owner) => _landsThisTurn[owner] = 0;
}
