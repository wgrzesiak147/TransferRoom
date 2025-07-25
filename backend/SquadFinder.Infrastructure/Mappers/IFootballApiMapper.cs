using SquadFinder.Api.Dtos;
using SquadFinder.Infrastructure.Models;

namespace SquadFinder.Infrastructure.Mappers
{
    public interface IFootballApiMapper
    {
        SquadDto MapToSquadDto(int teamId, string teamName, List<ApiSquadPlayer> players);
    }
}