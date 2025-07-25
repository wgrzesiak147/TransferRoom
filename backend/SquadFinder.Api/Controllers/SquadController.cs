using Microsoft.AspNetCore.Mvc;
using SquadFinder.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class SquadController : ControllerBase
{
    private readonly IFootballApiService _footballApiService;
    private readonly ILogger<SquadController> _logger;

    public SquadController(IFootballApiService footballApiService, ILogger<SquadController> logger)
    {
        _footballApiService = footballApiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetSquad([FromQuery] string team)
    {
        if (string.IsNullOrWhiteSpace(team))
        {
            _logger.LogWarning("GetSquad called without team parameter");
            return BadRequest(new { message = "Team query parameter is required." });
        }

        var result = await _footballApiService.GetTeamSquadAsync(team);

        if (result.IsFailed)
        {
            _logger.LogError("Failed to get squad for team {Team}. Errors: {Errors}", team, string.Join(", ", result.Errors.Select(e => e.Message)));

            var notFoundError = result.Errors.FirstOrDefault(e => e.Message.Contains("not found"));
            if (notFoundError != null)
            {
                return NotFound(new { message = notFoundError.Message });
            }

            return StatusCode(500, new { message = "An error occurred while retrieving the squad." });
        }

        return Ok(result.Value);
    }
}