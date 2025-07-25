namespace SquadFinder.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SquadFinder.Application.Interfaces;

    [ApiController]
    [Route("api/[controller]")]
    public class SquadController : ControllerBase
    {
        private readonly IFootballApiService _footballApiService;

        public SquadController(IFootballApiService footballApiService)
        {
            _footballApiService = footballApiService;
        }

        [HttpGet("{teamName}")]
        public async Task<IActionResult> GetSquad(string teamName)
        {
            var squad = await _footballApiService.GetTeamSquadAsync(teamName);

            if (squad == null)
            {
                return NotFound(new { message = $"Team '{teamName}' not found" });
            }

            return Ok(squad);
        }
    }

}
