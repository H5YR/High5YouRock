using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.ViewComponents.Footer
{
    public class SponsorsViewComponent : ViewComponent
    {
        private readonly ILogger<SponsorsViewComponent> _logger;
        public SponsorsViewComponent(ILogger<SponsorsViewComponent> logger)
        {
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }

    }
}
