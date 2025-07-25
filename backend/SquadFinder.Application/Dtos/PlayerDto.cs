namespace SquadFinder.Api.Dtos
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string Photo { get; set; } = null!; // URL to profile picture
    }
}
