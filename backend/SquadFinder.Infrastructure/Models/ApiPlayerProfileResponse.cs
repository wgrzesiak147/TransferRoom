using System.Text.Json.Serialization;

namespace SquadFinder.Infrastructure.Models
{
    public class ApiPlayerProfileResponse
    {
        [JsonPropertyName("response")]
        public List<ApiPlayerProfileEntry> Response { get; set; } = new();
    }

    public class ApiPlayerProfileEntry
    {
        [JsonPropertyName("player")]
        public ApiPlayerProfile Player { get; set; } = new();
    }

    public class ApiPlayerProfile
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
