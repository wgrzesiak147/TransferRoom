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

        [HttpGet]
        public async Task<IActionResult> GetSquad([FromQuery] string team)
        {
            var squad = await _footballApiService.GetTeamSquadAsync(team);

            if (squad == null)
            {
                return NotFound(new { message = $"Team '{team}' not found" });
            }

            return Ok(squad);
        }
    }

}
