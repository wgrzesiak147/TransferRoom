using System.Text.Json.Serialization;

namespace SquadFinder.Infrastructure.Models
{
    public class ApiTeamsResponse
    {
        [JsonPropertyName("response")]
        public List<ApiTeamEntry> Response { get; set; } = new();
    }

    public class ApiTeamEntry
    {
        [JsonPropertyName("team")]
        public ApiTeam Team { get; set; } = new();
    }

    public class ApiTeam
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
