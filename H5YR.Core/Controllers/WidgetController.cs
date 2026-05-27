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

        [HttpGet("embed.js")]
        [ResponseCache(Duration = 3600)]
        public IActionResult EmbedScript()
        {
            var js = @"(function(){
  var el=document.currentScript;
  var opts=el.dataset||{};
  var scriptSrc=el.src;
  var base=scriptSrc.substring(0,scriptSrc.indexOf('/widget/embed.js'));
  var url=opts.url||window.location.href;
  var params='url='+encodeURIComponent(url);
  if(opts.bgcolor)params+='&bgcolor='+encodeURIComponent(opts.bgcolor);
  if(opts.color)params+='&color='+encodeURIComponent(opts.color);
  if(opts.accent)params+='&accent='+encodeURIComponent(opts.accent);
  var w=opts.width||'320px';
  var h=opts.height||'350px';
  var iframe=document.createElement('iframe');
  iframe.src=base+'/widget?'+params;
  iframe.style.cssText='border:none;width:'+w+';height:'+h+';';
  iframe.title='Give a High 5';
  iframe.loading='lazy';
  el.parentNode.insertBefore(iframe,el.nextSibling);
})();";
            return Content(js, "application/javascript");
        }

        [HttpGet]
        public IActionResult Index([FromQuery] string url, [FromQuery] string bgcolor, [FromQuery] string color, [FromQuery] string accent)
        {
            ViewBag.TargetUrl = url ?? "";
            ViewBag.BaseUrl = _widgetSettings.Value.SiteBaseUrl?.TrimEnd('/') ?? "https://h5yr.com";
            ViewBag.BgColor = SanitizeHexColor(bgcolor);
            ViewBag.Color = SanitizeHexColor(color);
            ViewBag.Accent = SanitizeHexColor(accent);
            return View("~/Views/Widget/Index.cshtml");
        }

        private static string SanitizeHexColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            hex = hex.TrimStart('#');
            if (hex.Length is 3 or 6 && System.Text.RegularExpressions.Regex.IsMatch(hex, "^[0-9a-fA-F]+$"))
                return "#" + hex;
            return null;
        }
    }
}
