namespace H5YR.Core.Settings;

public class WidgetSettings
{
    public string? GitHubClientId { get; set; }
    public string? GitHubClientSecret { get; set; }
    public string? GoogleClientId { get; set; }
    public string? GoogleClientSecret { get; set; }
    public string? JwtSigningKey { get; set; }
    public int RateLimitPerSitePerDay { get; set; } = 3;
    public string? SiteBaseUrl { get; set; }
}
