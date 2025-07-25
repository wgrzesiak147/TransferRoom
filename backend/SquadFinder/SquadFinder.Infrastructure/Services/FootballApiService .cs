using Microsoft.Extensions.Configuration;
using SquadFinder.Api.Dtos;
using SquadFinder.Application.Interfaces;
using SquadFinder.Infrastructure.Mappers;
using SquadFinder.Infrastructure.Models;
using System.Text.Json;

public class FootballApiService : IFootballApiService
{
    private readonly HttpClient _httpClient;
    private readonly IFootballApiMapper _mapper;
    private readonly string _apiKey;
    private const int FootballApiPremierLeagueId = 39;

    public FootballApiService(HttpClient httpClient, IConfiguration configuration, IFootballApiMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _apiKey = configuration["ApiFootball:ApiKey"] ?? throw new ArgumentNullException("Api key not configured");
        _httpClient.DefaultRequestHeaders.Add("x-apisports-key", _apiKey);
    }

    public async Task<SquadDto?> GetTeamSquadAsync(string teamNameOrNickname)
    {
        // Step 1: Get teams
        var teamsResponse = await _httpClient.GetAsync($"teams?league={FootballApiPremierLeagueId}&season=2021");
        if (!teamsResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var teamsData = await JsonSerializer.DeserializeAsync<ApiTeamsResponse>(
            await teamsResponse.Content.ReadAsStreamAsync());

        var matchedTeam = teamsData?.Response
            .Select(t => t.Team)
            .FirstOrDefault(t =>
                string.Equals(t.Name, teamNameOrNickname, StringComparison.OrdinalIgnoreCase) ||
                (t.Nickname != null && t.Nickname.ToLower().Contains(teamNameOrNickname.ToLower()))
            );

        if (matchedTeam == null)
        {
            return null;
        }

        // Step 2: Get squad
        var squadResponse = await _httpClient.GetAsync($"players/squads?team={matchedTeam.Id}");
        if (!squadResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var squadData = await JsonSerializer.DeserializeAsync<ApiSquadResponse>(
            await squadResponse.Content.ReadAsStreamAsync());

        var players = squadData?.Response
            .SelectMany(r => r.Players)
            .ToList() ?? new List<ApiSquadPlayer>();

        return _mapper.MapToSquadDto(matchedTeam.Id, matchedTeam.Name, players);
    }
}