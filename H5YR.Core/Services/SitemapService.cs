using H5YR.Core.Models.ViewModels;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace H5YR.Core.Services;

/// <summary>
/// Service for generating XML sitemaps from Umbraco content
/// </summary>
public class SitemapService : ISitemapService
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IPublishedContentQuery _contentQuery;
    private readonly IPublishedValueFallback _publishedValueFallback;
    private readonly IPublishedUrlProvider _publishedUrlProvider;

    public SitemapService(
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedContentQuery contentQuery,
        IPublishedValueFallback publishedValueFallback,
        IPublishedUrlProvider publishedUrlProvider)
    {
        _umbracoContextFactory = umbracoContextFactory;
        _contentQuery = contentQuery;
        _publishedValueFallback = publishedValueFallback;
        _publishedUrlProvider = publishedUrlProvider;
    }

    /// <summary>
    /// Gets all pages that should be included in the sitemap
    /// </summary>
    public async Task<IEnumerable<SitemapUrlViewModel>> GetSitemapPagesAsync()
    {
        using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();

        var allPages = new List<IPublishedContent>();
        var rootNodes = _contentQuery.ContentAtRoot();

        if (rootNodes != null)
        {
            foreach (var root in rootNodes)
            {
                if (root != null)
                {
                    allPages.Add(root);
                    GetAllPages(root, allPages);
                }
            }
        }

        var visiblePages = allPages.Where(x => x.IsVisible(_publishedValueFallback));

        var sitemapUrls = visiblePages.Select(page => new SitemapUrlViewModel
        {
            Url = page.Url(_publishedUrlProvider, mode: UrlMode.Absolute),
            LastModified = page.UpdateDate.ToString("yyyy-MM-dd"),
            ChangeFrequency = page.Level == 1 ? "daily" : "weekly",
            Priority = page.Level == 1 ? "1.0" : page.Level == 2 ? "0.8" : "0.6"
        }).ToList();

        return await Task.FromResult(sitemapUrls);
    }

    /// <summary>
    /// Recursively collects all visible child pages
    /// </summary>
    private void GetAllPages(IPublishedContent node, List<IPublishedContent> pages)
    {
        foreach (var child in node.Children.Where(x => x.IsVisible(_publishedValueFallback)))
        {
            pages.Add(child);
            GetAllPages(child, pages);
        }
    }
}
