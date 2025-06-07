namespace GatheringTheMagic.Application.Services
{
    public interface IGameService
    {
        GameDto StartNewGame();
        CardDrawDto DrawCard();
        PlayResultDto PlayCard(Guid instanceId);
    }

    public record GameDto(
        int DeckCount,
        IEnumerable<CardDto> Hand
    );

    public record CardDto(
        string Name,
        object Types,    // use the enum or a string as you prefer
        Guid InstanceId
    );

    public record CardDrawDto(
        int DeckCount,
        CardDto? DrawnCard
    );

    public record PlayResultDto(
        IEnumerable<CardDto> Hand,
        IEnumerable<CardDto> Battlefield
    );
}
