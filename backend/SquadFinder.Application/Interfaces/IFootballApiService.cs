using FluentResults;
using SquadFinder.Api.Dtos;

namespace SquadFinder.Application.Interfaces
{
    public interface IFootballApiService
    {
        public Task<Result<SquadDto>> GetTeamSquadAsync(string teamNameOrNickname, int season);
    }
}
