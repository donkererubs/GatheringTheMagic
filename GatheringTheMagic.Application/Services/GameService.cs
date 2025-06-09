using GatheringTheMagic.Domain.Entities;

namespace GatheringTheMagic.Application.Services;

public class GameService : IGameService
{
    private readonly Game _game;

    public GameService(Game game)
    {
        _game = game;
    }

    public GameDto StartNewGame()
    {
        _game.Reset();
        var deckCount = _game.PlayerDeck.Cards.Count;
        var hand = _game.PlayerHand.Select(ToDto);
        return new GameDto(deckCount, hand);
    }

    public CardDrawDto DrawCard()
    {
        var card = _game.DrawCard();
        var deckCount = _game.PlayerDeck.Cards.Count;
        CardDto? drawn = card is null ? null : ToDto(card);
        return new CardDrawDto(deckCount, drawn);
    }

    public PlayResultDto PlayCard(Guid instanceId)
    {
        var card = _game.PlayerHand.FirstOrDefault(ci => ci.Id == instanceId);
        if (card is null)
            throw new InvalidOperationException($"No card {instanceId} in hand.");

        _game.PlayCard(card);

        var hand = _game.PlayerHand.Select(ToDto);
        var battlefield = _game.PlayerBattlefield.Select(ToDto);
        return new PlayResultDto(hand, battlefield);
    }

    public GameStateDto AdvancePhase()
    {
        _game.AdvancePhase();
        return BuildStateDto();
    }

    public GameStateDto AdvancePhase(int gameId)
    {
        // If you later support multiple games, lookup by gameId here.
        // For now, just advance our single _game.
        _game.AdvancePhase();
        return BuildStateDto();
    }

    public GameStateDto NextTurn()
    {
        _game.NextTurn();
        return BuildStateDto();
    }

    public GameStateDto GetGameState()
        => BuildStateDto();

    private GameStateDto BuildStateDto() => new(
        ActivePlayer: _game.ActivePlayer.ToString(),
        CurrentPhase: _game.CurrentPhase.ToString(),
        PlayerDeckCount: _game.PlayerDeck.Cards.Count,
        PlayerHand: _game.PlayerHand.Select(ToDto),
        PlayerBattlefield: _game.PlayerBattlefield.Select(ToDto),
        PlayerGraveyard: _game.PlayerGraveyard.Select(ToDto),
        OpponentDeckCount: _game.OpponentDeck.Cards.Count,
        OpponentHand: _game.OpponentHand.Select(ToDto),
        OpponentBattlefield: _game.OpponentBattlefield.Select(ToDto),
        OpponentGraveyard: _game.OpponentGraveyard.Select(ToDto),
        PriorityHolder: _game.PriorityHolder.ToString()
    );


    private static CardDto ToDto(CardInstance ci) => new(ci.Definition.Name, ci.Definition.Types, ci.Id);

    public Game? GetGame(Guid gameId)
    {
        // you only have one game in-memory today,
        // so just ignore the id and return it:
        return _game;
    }

    public void SaveGame(Game game)
    {
        // no-op: your _game instance is already mutated in-place
    }
}
