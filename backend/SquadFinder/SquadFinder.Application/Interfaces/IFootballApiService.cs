using SquadFinder.Api.Dtos;

namespace SquadFinder.Application.Interfaces
{
    public interface IFootballApiService
    {
        Task<SquadDto?> GetTeamSquadAsync(string teamNameOrNickname);
    }
}
