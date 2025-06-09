using GatheringTheMagic.Domain.Entities;
using GatheringTheMagic.Domain.Enums;

namespace GatheringTheMagic.Application.Services;

public interface IGameService
{
    GameDto StartNewGame();
    //GameStateDto StartNewGame(Guid gameId);
    CardDrawDto DrawCard();
    PlayResultDto PlayCard(Guid instanceId);

    /// <summary>Advance to the next phase, executing any automatic step logic.</summary>
    GameStateDto AdvancePhase(int gameId);

    /// <summary>End the current player's turn immediately.</summary>
    GameStateDto NextTurn();

    /// <summary>Get the full current state of the game.</summary>
    GameStateDto GetGameState();

    Game? GetGame(Guid gameId);
    void SaveGame(Game game);

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


// ── New DTO for full game state ──────────────────────────────────────── 
public record GameStateDto(
    string ActivePlayer,
    string CurrentPhase,
    int PlayerDeckCount,
    IEnumerable<CardDto> PlayerHand,
    IEnumerable<CardDto> PlayerBattlefield,
    IEnumerable<CardDto> PlayerGraveyard,
    int OpponentDeckCount,
    IEnumerable<CardDto> OpponentHand,
    IEnumerable<CardDto> OpponentBattlefield,
    IEnumerable<CardDto> OpponentGraveyard,

    string PriorityHolder
);


