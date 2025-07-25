using FluentResults;
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

    public FootballApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IFootballApiMapper mapper,
        ILogger<FootballApiService> logger)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _logger = logger;

        _apiKey = configuration["ApiFootball:ApiKey"]
                  ?? throw new ArgumentNullException("Api key not configured");

        _httpClient.DefaultRequestHeaders.Add("x-apisports-key", _apiKey);
    }

    public async Task<Result<SquadDto>> GetTeamSquadAsync(string teamNameOrNickname)
    {
        try
        {
            _logger.LogInformation("Fetching teams for league {LeagueId} and season 2021", FootballApiPremierLeagueId);

            var teamsResponse = await _httpClient.GetAsync($"teams?league={FootballApiPremierLeagueId}&season=2021");
            if (!teamsResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch teams. Status code: {StatusCode}", teamsResponse.StatusCode);
                return Result.Fail<SquadDto>($"Failed to fetch teams. Status code: {teamsResponse.StatusCode}");
            }

            var teamsData = await JsonSerializer.DeserializeAsync<ApiTeamsResponse>(
                await teamsResponse.Content.ReadAsStreamAsync());

            var matchedTeam = teamsData?.Response
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

            _logger.LogInformation("Successfully fetched squad for team {TeamId}", matchedTeam.Id);

            return Result.Ok(squadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching squad for team '{TeamName}'", teamNameOrNickname);
            return Result.Fail<SquadDto>($"Unexpected error occurred: {ex.Message}");
        }
    }
}