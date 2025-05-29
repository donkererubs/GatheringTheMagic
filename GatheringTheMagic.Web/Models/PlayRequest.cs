namespace GatheringTheMagic.Web.Models
{
    /// <summary>
    /// Payload for the /api/game/play endpoint.
    /// </summary>
    public record PlayRequest(int Index, bool TappedOnEntry = false, bool ForceLand = false);
}
