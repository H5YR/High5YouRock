using H5YR.Core.Services;
using H5YR.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace H5YR.Core.Controllers.API
{
    [Route("api/widget/auth")]
    public class WidgetAuthController : Controller
    {
        private readonly ILogger<WidgetAuthController> _logger;
        private readonly IOptions<WidgetSettings> _widgetSettings;
        private readonly IWidgetJwtService _jwtService;
        private readonly IHttpClientFactory _httpClientFactory;

        public WidgetAuthController(
            ILogger<WidgetAuthController> logger,
            IOptions<WidgetSettings> widgetSettings,
            IWidgetJwtService jwtService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _widgetSettings = widgetSettings;
            _jwtService = jwtService;
            _httpClientFactory = httpClientFactory;
        }

        private string BaseUrl => _widgetSettings.Value.SiteBaseUrl?.TrimEnd('/') ?? "https://h5yr.com";

        // ─── GitHub OAuth ───────────────────────────────────────────

        [HttpGet("github")]
        public IActionResult GitHubLogin()
        {
            var clientId = _widgetSettings.Value.GitHubClientId;
            // Always use the main H5YR domain for OAuth callbacks
            var redirectUri = "https://h5yr.com/api/widget/auth/github/callback";
            var authUrl = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=read:user";
            return Redirect(authUrl);
        }

        [HttpGet("github/callback")]
        public async Task<IActionResult> GitHubCallback([FromQuery] string code, [FromQuery] string? error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return PostMessageAndClose(null, "GitHub sign-in was cancelled.");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
                tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _widgetSettings.Value.GitHubClientId!,
                    ["client_secret"] = _widgetSettings.Value.GitHubClientSecret!,
                    ["code"] = code
                });

                var tokenResponse = await httpClient.SendAsync(tokenRequest);
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonDocument.Parse(tokenJson);

                if (!tokenData.RootElement.TryGetProperty("access_token", out var accessTokenEl))
                {
                    _logger.LogWarning("GitHub OAuth: no access_token in response: {Json}", tokenJson);
                    return PostMessageAndClose(null, "Failed to authenticate with GitHub.");
                }

                var accessToken = accessTokenEl.GetString();
                if (string.IsNullOrEmpty(accessToken))
                    return PostMessageAndClose(null, "Failed to authenticate with GitHub.");

                var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
                userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                userRequest.Headers.UserAgent.ParseAdd("H5YR-Widget/1.0");

                var userResponse = await httpClient.SendAsync(userRequest);
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonDocument.Parse(userJson);

                var userId = userData.RootElement.GetProperty("id").GetInt64().ToString();
                var displayName = userData.RootElement.TryGetProperty("name", out var nameEl) && nameEl.ValueKind != JsonValueKind.Null
                    ? nameEl.GetString() ?? ""
                    : userData.RootElement.GetProperty("login").GetString() ?? "";
                var avatarUrl = userData.RootElement.GetProperty("avatar_url").GetString() ?? "";
                var profileUrl = userData.RootElement.GetProperty("html_url").GetString() ?? "";

                var jwt = _jwtService.GenerateToken("github", userId, displayName, avatarUrl, profileUrl);

                return PostMessageAndClose(jwt, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub OAuth callback failed");
                return PostMessageAndClose(null, "Authentication failed.");
            }
        }

        // ─── Google OAuth ───────────────────────────────────────────

        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var clientId = _widgetSettings.Value.GoogleClientId;
            var redirectUri = "https://h5yr.com/api/widget/auth/google/callback";
            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope=openid%20profile&access_type=online";
            return Redirect(authUrl);
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? code, [FromQuery] string? error)
        {
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
            {
                return PostMessageAndClose(null, "Google sign-in was cancelled.");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var redirectUri = $"{BaseUrl}/api/widget/auth/google/callback";
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
                tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _widgetSettings.Value.GoogleClientId!,
                    ["client_secret"] = _widgetSettings.Value.GoogleClientSecret!,
                    ["code"] = code,
                    ["grant_type"] = "authorization_code",
                    ["redirect_uri"] = redirectUri
                });

                var tokenResponse = await httpClient.SendAsync(tokenRequest);
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonDocument.Parse(tokenJson);

                if (!tokenData.RootElement.TryGetProperty("access_token", out var accessTokenEl))
                    return PostMessageAndClose(null, "Failed to authenticate with Google.");

                var accessToken = accessTokenEl.GetString();
                if (string.IsNullOrEmpty(accessToken))
                    return PostMessageAndClose(null, "Failed to authenticate with Google.");

                var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
                userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var userResponse = await httpClient.SendAsync(userRequest);
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonDocument.Parse(userJson);

                var userId = userData.RootElement.GetProperty("id").GetString() ?? "";
                var displayName = userData.RootElement.TryGetProperty("name", out var nameEl) ? nameEl.GetString() ?? "" : "";
                var avatarUrl = userData.RootElement.TryGetProperty("picture", out var picEl) ? picEl.GetString() ?? "" : "";

                var jwt = _jwtService.GenerateToken("google", userId, displayName, avatarUrl, "");

                return PostMessageAndClose(jwt, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google OAuth callback failed");
                return PostMessageAndClose(null, "Authentication failed.");
            }
        }

        // ─── Shared helper ──────────────────────────────────────────

        /// <summary>
        /// Returns a minimal HTML page that sends the token (or error) back to the
        /// widget iframe via postMessage, then closes the popup.
        /// </summary>
        private ContentResult PostMessageAndClose(string? token, string? errorMessage)
        {
            var payload = token != null
                ? $"{{ type: 'h5yr_auth', token: {JsonSerializer.Serialize(token)} }}"
                : $"{{ type: 'h5yr_auth', error: {JsonSerializer.Serialize(errorMessage ?? "Unknown error")} }}";

            // Send postMessage to the configured base URL origin for security
            var targetOrigin = BaseUrl;

            var html = $@"<!DOCTYPE html>
<html>
<head><title>Signing in...</title></head>
<body>
<script>
  try {{
    if (window.opener) {{
      window.opener.postMessage({payload}, {JsonSerializer.Serialize(targetOrigin)});
    }}
  }} catch(e) {{
    console.error('Failed to send auth result:', e);
  }}
  setTimeout(function() {{ window.close(); }}, 500);
</script>
<p>Signing in, please wait&hellip;</p>
</body>
</html>";

            return Content(html, "text/html");
        }
    }
}
