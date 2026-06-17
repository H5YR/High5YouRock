namespace H5YR.Core.Models.ViewModels;

/// <summary>
/// View model for a sitemap URL entry
/// </summary>
public class SitemapUrlViewModel
{
    public string Url { get; set; } = string.Empty;
    public string LastModified { get; set; } = string.Empty;
    public string ChangeFrequency { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}
