using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Controllers.API
{
    [Route("api/widget/h5yr")]
    public class WidgetH5yrController : Controller
    {
        private readonly ILogger<WidgetH5yrController> _logger;
        private readonly IWidgetH5yrService _widgetH5yrService;
        private readonly IWidgetJwtService _jwtService;

        public WidgetH5yrController(
            ILogger<WidgetH5yrController> logger,
            IWidgetH5yrService widgetH5yrService,
            IWidgetJwtService jwtService)
        {
            _logger = logger;
            _widgetH5yrService = widgetH5yrService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public IActionResult Submit([FromBody] WidgetSubmitRequest request)
        {
            // Validate JWT token
            var principal = _jwtService.ValidateToken(request.Token);
            if (principal == null)
            {
                return Unauthorized(new { error = "Invalid or expired token. Please sign in again." });
            }

            var authProvider = principal.FindFirst("auth_provider")?.Value ?? "";
            var externalUserId = principal.FindFirst("external_user_id")?.Value ?? "";
            var displayName = principal.FindFirst("display_name")?.Value ?? "";
            var avatarUrl = principal.FindFirst("avatar_url")?.Value ?? "";
            var profileUrl = principal.FindFirst("profile_url")?.Value ?? "";

            if (string.IsNullOrEmpty(request.TargetUrl))
            {
                return BadRequest(new { error = "Target URL is required." });
            }

            var result = _widgetH5yrService.Submit(
                authProvider, externalUserId, displayName, avatarUrl, profileUrl, request.TargetUrl);

            if (result == null)
            {
                // Either rate limited or invalid URL
                if (Uri.TryCreate(request.TargetUrl, UriKind.Absolute, out var uri))
                {
                    var remaining = _widgetH5yrService.GetRemainingSubmissions(
                        externalUserId, authProvider, uri.Host.ToLowerInvariant());
                    if (remaining <= 0)
                    {
                        return StatusCode(429, new { error = "You've used all your #h5yr submissions for this site today. Come back tomorrow!" });
                    }
                }

                return BadRequest(new { error = "Invalid URL. Please provide a valid http or https URL." });
            }

            return Ok(new
            {
                success = true,
                message = "Your #h5yr has been submitted! It will appear in the feed shortly."
            });
        }

        [HttpGet("status")]
        public IActionResult GetStatus([FromQuery] string token, [FromQuery] string url)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Ok(new { authenticated = false });
            }

            var principal = _jwtService.ValidateToken(token);
            if (principal == null)
            {
                return Ok(new { authenticated = false });
            }

            var authProvider = principal.FindFirst("auth_provider")?.Value ?? "";
            var externalUserId = principal.FindFirst("external_user_id")?.Value ?? "";
            var displayName = principal.FindFirst("display_name")?.Value ?? "";
            var avatarUrl = principal.FindFirst("avatar_url")?.Value ?? "";

            int remaining = 3; // default
            if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                remaining = _widgetH5yrService.GetRemainingSubmissions(
                    externalUserId, authProvider, uri.Host.ToLowerInvariant());
            }

            return Ok(new
            {
                authenticated = true,
                displayName,
                avatarUrl,
                authProvider,
                remaining
            });
        }
    }

    public class WidgetSubmitRequest
    {
        public string Token { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
    }
}
