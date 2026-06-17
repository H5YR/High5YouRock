using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace H5YR.Core.Controllers;

/// <summary>
/// Controller for serving the XML sitemap
/// </summary>
[Route("sitemap.xml")]
public class SitemapController : Controller
{
    private readonly ISitemapService _sitemapService;

    public SitemapController(ISitemapService sitemapService)
    {
        _sitemapService = sitemapService;
    }

    /// <summary>
    /// Returns the XML sitemap
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pages = await _sitemapService.GetSitemapPagesAsync();
        return View("~/Views/Sitemap.cshtml", pages);
    }
}
