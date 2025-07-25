using FluentResults;
using SquadFinder.Api.Dtos;

namespace SquadFinder.Application.Interfaces
{
    public interface IFootballApiService
    {
        Task<Result<SquadDto>> GetTeamSquadAsync(string teamNameOrNickname);
    }
}
