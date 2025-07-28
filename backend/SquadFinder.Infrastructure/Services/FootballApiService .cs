using FluentResults;
using Microsoft.Extensions.Caching.Memory;
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
    private const int FootballApiPremierLeagueId = 39;
    private readonly IMemoryCache _cache;

    public FootballApiService(HttpClient httpClient, IFootballApiMapper mapper, IMemoryCache cache, ILogger<FootballApiService> logger)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Enriching squad player with birth data as its not included by default
    /// </summary>
    /// <param name="basePlayer"></param>
    /// <param name="detail"></param>
    /// <returns></returns>
    private ApiSquadPlayer EnrichSquadPlayer(ApiSquadPlayer basePlayer, ApiPlayerProfile detail)
    {
        if (detail != null)
        {
            basePlayer.Birth = detail.Birth;
        }

        return basePlayer;
    }

    /// <summary>
    /// Getting team squad async by team name and season. Caching responses for 1 hour
    /// </summary>
    /// <param name="teamName"></param>
    /// <returns></returns>
    public async Task<Result<SquadDto>> GetTeamSquadAsync(string teamName, int season)
    {
        try
        {
            var normalizedTeamName = teamName.Trim().ToLowerInvariant();
            var squadCacheKey = $"squad:{normalizedTeamName}";
            var teamsCacheKey = $"teams:league:{FootballApiPremierLeagueId}:season:{season}";

            // Try to get squad from cache first
            if (_cache.TryGetValue(squadCacheKey, out SquadDto cachedSquad))
            {
                _logger.LogInformation("Squad for team '{TeamName}' retrieved from cache", teamName);
                return Result.Ok(cachedSquad);
            }

            // Try to get teams from cache
            List<ApiTeamEntry>? teamsList;
            if (!_cache.TryGetValue(teamsCacheKey, out teamsList))
            {
                _logger.LogInformation("Fetching teams from external API for league {LeagueId} and season {Season}", FootballApiPremierLeagueId, season);

                var teamsResponse = await _httpClient.GetAsync($"teams?league={FootballApiPremierLeagueId}&season={season}");
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
                    string.Equals(t.Name, teamName, StringComparison.OrdinalIgnoreCase) ||
                    t.Name.Contains(teamName, StringComparison.OrdinalIgnoreCase));

            if (matchedTeam == null)
            {
                _logger.LogWarning("Team '{TeamName}' not found in league {LeagueId}", teamName, FootballApiPremierLeagueId);
                return Result.Fail<SquadDto>($"Team '{teamName}' not found.");
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


            // Fetch full player details in parallel
            await EnrichPlayersWithExtraData(season, players);

            var squadDto = _mapper.MapToSquadDto(matchedTeam.Id, matchedTeam.Name, players);

            _cache.Set(squadCacheKey, squadDto, TimeSpan.FromHours(1));
            _logger.LogInformation("Successfully fetched and cached squad for team {TeamId}", matchedTeam.Id);

            return Result.Ok(squadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching squad for team '{TeamName}'", teamName);
            return Result.Fail<SquadDto>($"Unexpected error occurred: {ex.Message}");
        }

    }

    /// <summary>
    /// Needed to extend players with Birthdate
    /// </summary>
    /// <param name="season"></param>
    /// <param name="players"></param>
    /// <returns></returns>
    private async Task EnrichPlayersWithExtraData(int season, List<ApiSquadPlayer> players)
    {
        var enrichedPlayers = await Task.WhenAll(players.Select(async player =>
        {
            var playerCacheKey = $"player:{player.Id}";

            if (_cache.TryGetValue(playerCacheKey, out ApiPlayerProfile cachedPlayerProfile))
            {
                return EnrichSquadPlayer(player, cachedPlayerProfile);
            }

            var playerResponse = await _httpClient.GetAsync($"players?id={player.Id}&season={season}");
            if (!playerResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch details for player {PlayerId}. Status code: {StatusCode}", player.Id, playerResponse.StatusCode);
                return player; // fallback to original player data
            }

            var playerDetailWrapper = await JsonSerializer.DeserializeAsync<ApiPlayerProfileResponse>(
                await playerResponse.Content.ReadAsStreamAsync());

            var playerProfileResponse = playerDetailWrapper?.Response?.FirstOrDefault();

            if (playerProfileResponse != null)
            {
                _cache.Set(playerCacheKey, playerProfileResponse, TimeSpan.FromHours(6));
                return EnrichSquadPlayer(player, playerProfileResponse.Player);
            }

            return player;
        }));
    }
}