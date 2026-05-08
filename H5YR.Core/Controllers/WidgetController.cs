using H5YR.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace H5YR.Core.Controllers
{
    [Route("widget")]
    public class WidgetController : Controller
    {
        private readonly IOptions<WidgetSettings> _widgetSettings;

        public WidgetController(IOptions<WidgetSettings> widgetSettings)
        {
            _widgetSettings = widgetSettings;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] string url)
        {
            ViewBag.TargetUrl = url ?? "";
            ViewBag.BaseUrl = _widgetSettings.Value.SiteBaseUrl?.TrimEnd('/') ?? "https://h5yr.com";
            return View("~/Views/Widget/Index.cshtml");
        }
    }
}
