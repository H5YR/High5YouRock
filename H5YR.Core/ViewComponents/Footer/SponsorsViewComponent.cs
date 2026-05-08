using H5YR.Core.Models.ViewModels;
using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.ViewComponents.Footer
{
    public class SponsorsViewComponent : ViewComponent
    {
        private readonly ILogger<SponsorsViewComponent> _logger;
        private readonly HomeSettingsService _homeSettingsService;
        public SponsorsViewComponent(ILogger<SponsorsViewComponent> logger, HomeSettingsService homeSettingsService)
        {
            _logger = logger;
            _homeSettingsService = homeSettingsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var home = await _homeSettingsService.SettingsAsync();
            var model = new SponsorsViewModel
            {
                Sponsors = home?.Sponsors
            };
            return View(model);
        }

    }
}
