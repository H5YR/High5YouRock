using System.Net.Http.Headers;
using System.Text.Json;
using H5YR.Core.Services;
using H5YR.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace H5YR.Core.Controllers.API
{
    [Route("api/widget/auth")]
    public class WidgetAuthController : Controller
    {
        private readonly ILogger<WidgetAuthController> _logger;
        private readonly IOptions<WidgetSettings> _widgetSettings;
        private readonly IWidgetJwtService _jwtService;
        private static readonly HttpClient _httpClient = new();

        public WidgetAuthController(
            ILogger<WidgetAuthController> logger,
            IOptions<WidgetSettings> widgetSettings,
            IWidgetJwtService jwtService)
        {
            _logger = logger;
            _widgetSettings = widgetSettings;
            _jwtService = jwtService;
        }

        private string BaseUrl => _widgetSettings.Value.SiteBaseUrl?.TrimEnd('/') ?? "https://h5yr.com";

        // ─── GitHub OAuth ───────────────────────────────────────────

        [HttpGet("github")]
        public IActionResult GitHubLogin([FromQuery] string returnUrl)
        {
            var clientId = _widgetSettings.Value.GitHubClientId;
            var redirectUri = $"{BaseUrl}/api/widget/auth/github/callback";

            // Store the widget return URL in a cookie so we can redirect back after OAuth
            HttpContext.Response.Cookies.Append("h5yr_widget_return", returnUrl ?? "/",
                new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromMinutes(10) });

            var authUrl = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=read:user";

            return Redirect(authUrl);
        }

        [HttpGet("github/callback")]
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            try
            {
                // Exchange code for access token
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
                tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _widgetSettings.Value.GitHubClientId!,
                    ["client_secret"] = _widgetSettings.Value.GitHubClientSecret!,
                    ["code"] = code
                });

                var tokenResponse = await _httpClient.SendAsync(tokenRequest);
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonDocument.Parse(tokenJson);
                var accessToken = tokenData.RootElement.GetProperty("access_token").GetString();

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogWarning("GitHub OAuth: failed to get access token");
                    return BadRequest("Failed to authenticate with GitHub.");
                }

                // Get user info
                var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
                userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                userRequest.Headers.UserAgent.ParseAdd("H5YR-Widget/1.0");

                var userResponse = await _httpClient.SendAsync(userRequest);
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonDocument.Parse(userJson);

                var userId = userData.RootElement.GetProperty("id").GetInt64().ToString();
                var displayName = userData.RootElement.TryGetProperty("name", out var nameEl) && nameEl.ValueKind != JsonValueKind.Null
                    ? nameEl.GetString() ?? ""
                    : userData.RootElement.GetProperty("login").GetString() ?? "";
                var login = userData.RootElement.GetProperty("login").GetString() ?? "";
                var avatarUrl = userData.RootElement.GetProperty("avatar_url").GetString() ?? "";
                var profileUrl = userData.RootElement.GetProperty("html_url").GetString() ?? "";

                // Generate JWT
                var jwt = _jwtService.GenerateToken("github", userId, displayName, avatarUrl, profileUrl);

                // Redirect back to widget with token
                var returnUrl = HttpContext.Request.Cookies["h5yr_widget_return"] ?? "/";
                HttpContext.Response.Cookies.Delete("h5yr_widget_return");

                // Redirect to widget page with token as fragment (not query param, for security)
                return Redirect($"{returnUrl}{(returnUrl.Contains('?') ? '&' : '?')}h5yr_token={jwt}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub OAuth callback failed");
                return BadRequest("Authentication failed.");
            }
        }

        // ─── Google OAuth ───────────────────────────────────────────

        [HttpGet("google")]
        public IActionResult GoogleLogin([FromQuery] string returnUrl)
        {
            var clientId = _widgetSettings.Value.GoogleClientId;
            var redirectUri = $"{BaseUrl}/api/widget/auth/google/callback";

            HttpContext.Response.Cookies.Append("h5yr_widget_return", returnUrl ?? "/",
                new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromMinutes(10) });

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope=openid%20profile&access_type=online";

            return Redirect(authUrl);
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            try
            {
                // Exchange code for tokens
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
                tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _widgetSettings.Value.GoogleClientId!,
                    ["client_secret"] = _widgetSettings.Value.GoogleClientSecret!,
                    ["code"] = code,
                    ["grant_type"] = "authorization_code",
                    ["redirect_uri"] = $"{BaseUrl}/api/widget/auth/google/callback"
                });

                var tokenResponse = await _httpClient.SendAsync(tokenRequest);
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonDocument.Parse(tokenJson);
                var accessToken = tokenData.RootElement.GetProperty("access_token").GetString();

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogWarning("Google OAuth: failed to get access token");
                    return BadRequest("Failed to authenticate with Google.");
                }

                // Get user info
                var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
                userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var userResponse = await _httpClient.SendAsync(userRequest);
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonDocument.Parse(userJson);

                var userId = userData.RootElement.GetProperty("id").GetString() ?? "";
                var displayName = userData.RootElement.TryGetProperty("name", out var nameEl)
                    ? nameEl.GetString() ?? ""
                    : "";
                var avatarUrl = userData.RootElement.TryGetProperty("picture", out var picEl)
                    ? picEl.GetString() ?? ""
                    : "";
                // Google doesn't have a public profile URL like GitHub, so we'll use an empty string
                var profileUrl = "";

                // Generate JWT
                var jwt = _jwtService.GenerateToken("google", userId, displayName, avatarUrl, profileUrl);

                // Redirect back to widget
                var returnUrl = HttpContext.Request.Cookies["h5yr_widget_return"] ?? "/";
                HttpContext.Response.Cookies.Delete("h5yr_widget_return");

                return Redirect($"{returnUrl}{(returnUrl.Contains('?') ? '&' : '?')}h5yr_token={jwt}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google OAuth callback failed");
                return BadRequest("Authentication failed.");
            }
        }
    }
}
