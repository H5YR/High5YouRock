using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Controllers.API
{
  [Route("api/communitystats")]
  [ApiController]
  public class CommunityStatsController : ControllerBase
  {
    private readonly ILogger<CommunityStatsController> _logger;
    private readonly ICommunityStatsService _statsService;

    public CommunityStatsController(
        ILogger<CommunityStatsController> logger,
        ICommunityStatsService statsService)
    {
      _logger = logger;
      _statsService = statsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
      var stats = await _statsService.GetStats();
      return Ok(stats);
    }
  }
}
