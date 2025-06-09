using System.Text.Json.Serialization;

namespace GatheringTheMagic.Web.DTOs;

public class PhaseRequest
{
    [JsonPropertyName("gameId")]
    public int GameId { get; set; }
}