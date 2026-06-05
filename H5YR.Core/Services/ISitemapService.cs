using H5YR.Core.Models.ViewModels;

namespace H5YR.Core.Services;

/// <summary>
/// Service for generating XML sitemaps
/// </summary>
public interface ISitemapService
{
    /// <summary>
    /// Gets all pages that should be included in the sitemap
    /// </summary>
    Task<IEnumerable<SitemapUrlViewModel>> GetSitemapPagesAsync();
}
