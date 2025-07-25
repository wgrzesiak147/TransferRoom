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

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
    }

    public class ApiSquadResponse
    {
        [JsonPropertyName("response")]
        public List<ApiSquadTeamEntry> Response { get; set; } = new();
    }

    public class ApiSquadTeamEntry
    {
        [JsonPropertyName("players")]
        public List<ApiSquadPlayer> Players { get; set; } = new();
    }

    public class ApiSquadPlayer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("position")]
        public string Position { get; set; } = "";

        [JsonPropertyName("birth")]
        public ApiBirthInfo Birth { get; set; } = new();

        [JsonPropertyName("photo")]
        public string Photo { get; set; } = "";
    }

    public class ApiBirthInfo
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = "";
    }
}
