using SquadFinder.Api.Dtos;
using SquadFinder.Infrastructure.Models;

namespace SquadFinder.Infrastructure.Mappers
{
    public class FootballApiMapper : IFootballApiMapper
    {
        public SquadDto MapToSquadDto(int teamId, string teamName, List<ApiSquadPlayer> players)
        {
            var playerDtos = players.Select(p =>
            {
                var nameParts = p.Name.Split(' ');
                var firstName = nameParts.Length > 0 ? nameParts[0] : "";
                var lastName = nameParts.Length > 1 ? string.Join(' ', nameParts.Skip(1)) : "";

                return new PlayerDto
                {
                    PlayerId = p.Id,
                    FirstName = firstName,
                    LastName = lastName,
                    Position = p.Position,
                    DateOfBirth = DateTime.TryParse(p.Birth?.Date, out var dob) ? dob : null,
                    Photo = p.Photo
                };
            }).ToList();

            return new SquadDto
            {
                TeamId = teamId,
                TeamName = teamName,
                Players = playerDtos
            };
        }
    }
}
