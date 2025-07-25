namespace SquadFinder.Api.Dtos
{
    public class SquadDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public List<PlayerDto> Players { get; set; } = new();
    }
}
