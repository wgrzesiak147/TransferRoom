using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SquadFinder.Api.Dtos;
using SquadFinder.Application.Interfaces;
using SquadFinder.Infrastructure.Mappers;
using SquadFinder.Infrastructure.Models;
using System.Text.Json;

public class FootballApiService : IFootballApiService
{
    private readonly HttpClient _httpClient;
    private readonly IFootballApiMapper _mapper;
    private readonly ILogger<FootballApiService> _logger;
    private readonly string _apiKey;
    private const int FootballApiPremierLeagueId = 39;
    private readonly IMemoryCache _cache;

    public FootballApiService(HttpClient httpClient, IConfiguration configuration, IFootballApiMapper mapper, IMemoryCache cache, ILogger<FootballApiService> logger)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _cache = cache;
        _apiKey = configuration["ApiFootball:ApiKey"] ?? throw new ArgumentNullException("Api key not configured");
        _httpClient.DefaultRequestHeaders.Add("x-apisports-key", _apiKey);
        _logger = logger;
    }

    public async Task<Result<SquadDto>> GetTeamSquadAsync(string teamNameOrNickname)
    {
        try
        {
            var normalizedTeamName = teamNameOrNickname.Trim().ToLowerInvariant();
            var squadCacheKey = $"squad:{normalizedTeamName}";
            var teamsCacheKey = $"teams:league:{FootballApiPremierLeagueId}:season:2021";

            // Try to get squad from cache first
            if (_cache.TryGetValue(squadCacheKey, out SquadDto cachedSquad))
            {
                _logger.LogInformation("Squad for team '{TeamName}' retrieved from cache", teamNameOrNickname);
                return Result.Ok(cachedSquad);
            }

            // Try to get teams from cache
            List<ApiTeamEntry>? teamsList;
            if (!_cache.TryGetValue(teamsCacheKey, out teamsList))
            {
                _logger.LogInformation("Fetching teams from external API for league {LeagueId} and season 2021", FootballApiPremierLeagueId);

                var teamsResponse = await _httpClient.GetAsync($"teams?league={FootballApiPremierLeagueId}&season=2021");
                if (!teamsResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch teams. Status code: {StatusCode}", teamsResponse.StatusCode);
                    return Result.Fail<SquadDto>($"Failed to fetch teams. Status code: {teamsResponse.StatusCode}");
                }

                var teamsData = await JsonSerializer.DeserializeAsync<ApiTeamsResponse>(
                    await teamsResponse.Content.ReadAsStreamAsync());

                teamsList = teamsData?.Response ?? new List<ApiTeamEntry>();

                _cache.Set(teamsCacheKey, teamsList, TimeSpan.FromHours(1));
                _logger.LogInformation("Team list cached for 1 hour.");
            }

            var matchedTeam = teamsList
                .Select(t => t.Team)
                .FirstOrDefault(t =>
                    string.Equals(t.Name, teamNameOrNickname, StringComparison.OrdinalIgnoreCase) ||
                    (t.Nickname != null && t.Nickname.Contains(teamNameOrNickname, StringComparison.OrdinalIgnoreCase))
                );

            if (matchedTeam == null)
            {
                _logger.LogWarning("Team '{TeamName}' not found in league {LeagueId}", teamNameOrNickname, FootballApiPremierLeagueId);
                return Result.Fail<SquadDto>($"Team '{teamNameOrNickname}' not found.");
            }

            _logger.LogInformation("Fetching squad for team {TeamId} - {TeamName}", matchedTeam.Id, matchedTeam.Name);

            var squadResponse = await _httpClient.GetAsync($"players/squads?team={matchedTeam.Id}");
            if (!squadResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch squad for team id {TeamId}. Status code: {StatusCode}", matchedTeam.Id, squadResponse.StatusCode);
                return Result.Fail<SquadDto>($"Failed to fetch squad for team id {matchedTeam.Id}. Status code: {squadResponse.StatusCode}");
            }

            var squadData = await JsonSerializer.DeserializeAsync<ApiSquadResponse>(
                await squadResponse.Content.ReadAsStreamAsync());

            var players = squadData?.Response
                .SelectMany(r => r.Players)
                .ToList() ?? new List<ApiSquadPlayer>();

            var squadDto = _mapper.MapToSquadDto(matchedTeam.Id, matchedTeam.Name, players);

            _cache.Set(squadCacheKey, squadDto, TimeSpan.FromHours(1));
            _logger.LogInformation("Successfully fetched and cached squad for team {TeamId}", matchedTeam.Id);

            return Result.Ok(squadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching squad for team '{TeamName}'", teamNameOrNickname);
            return Result.Fail<SquadDto>($"Unexpected error occurred: {ex.Message}");
        }
    }
}