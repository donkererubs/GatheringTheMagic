namespace GatheringTheMagic.Domain.Enums;

/// <summary>
/// Represents various in‐play states a Magic card can have simultaneously.
/// </summary>
[Flags]
public enum CardStatus
{
    None = 0,
    Tapped = 1 << 0,  // card is turned sideways
    SummoningSickness = 1 << 1,  // creature can’t attack or {T} yet
    Flipped = 1 << 2,  // flipped via a flip card’s ability
    FaceDown = 1 << 3,  // face-down (e.g. morph)
    Enchanted = 1 << 4,  // has at least one aura attached
    Equipped = 1 << 5,  // has at least one equipment attached
    PhasedOut = 1 << 6,  // currently phased out
    Transformed = 1 << 7
}
