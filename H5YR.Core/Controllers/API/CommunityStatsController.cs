using H5YR.Core.Services;
using H5YR.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace H5YR.Core.Controllers.API
{
  /// <summary>
  /// API controller for community statistics data
  /// </summary>
  [Route("api/communitystats")]
  [ApiController]
  public class CommunityStatsController : ControllerBase
  {
    private readonly ILogger<CommunityStatsController> _logger;
    private readonly IOptions<APISettings> _apiSettings;
    private readonly ICommunityStatsService _statsService;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Gets whether the application is running in offline mode
    /// </summary>
    protected bool IsOffline => _apiSettings.Value.Offline == "true";

    /// <summary>
    /// Initializes a new instance of the CommunityStatsController
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="apiSettings">API settings configuration</param>
    /// <param name="statsService">Community stats service</param>
    /// <param name="httpClientFactory">HTTP client factory for remote API calls</param>
    public CommunityStatsController(
        ILogger<CommunityStatsController> logger,
        IOptions<APISettings> apiSettings,
        ICommunityStatsService statsService,
        IHttpClientFactory httpClientFactory)
    {
      _logger = logger;
      _apiSettings = apiSettings;
      _statsService = statsService;
      _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Gets community statistics including activity timeline and top contributors
    /// </summary>
    /// <returns>Community statistics JSON data</returns>
    /// <remarks>
    /// When offline mode is enabled (APISettings.Offline = "true"), fetches data from the live site at https://h5yr.com/api/communitystats.
    /// Otherwise, uses the local CommunityStatsService to generate statistics from the database.
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
      if (IsOffline && !string.Equals(Request.Host.Host, "h5yr.com", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          var client = _httpClientFactory.CreateClient();
          var response = await client.GetAsync("https://h5yr.com/api/communitystats");

          if (response.IsSuccessStatusCode)
          {
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
          }

          _logger.LogWarning("Failed to fetch stats from live site: {StatusCode}", response.StatusCode);
          return StatusCode((int)response.StatusCode);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error fetching community stats from live site");
          return StatusCode(500, new { error = "Failed to fetch community stats" });
        }
      }

      var stats = await _statsService.GetStats();
      return Ok(stats);
    }
  }
}
