using h5yr.Core.Services;
using Microsoft.AspNetCore.Mvc;
using h5yr.Settings;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.ViewComponents;

public class MastodonViewComponent : ViewComponent {

    private readonly ILogger<MastodonViewComponent> _logger;
    private readonly IOptions<APISettings> _apiSettings;
    private readonly IMastodonService _mastodonService;

    protected bool IsOffline => _apiSettings.Value.Offline == "true";

    protected bool CreateOfflineFile => _apiSettings.Value.CreateOfflineFile == "true";

    public MastodonViewComponent(ILogger<MastodonViewComponent> logger, IOptions<APISettings> apiSettings, IMastodonService mastodonService) {
        _logger = logger;
        _apiSettings = apiSettings;
        _mastodonService = mastodonService;
    }

    public async Task<IViewComponentResult> InvokeAsync() {

        // Get the statuses
        IReadOnlyList<MastodonStatus> statuses = await GetStatuses();

        // Initialize a new model for the view component
        MastodonModel model = new(statuses);

        // Return the view
        return View(model);

    }

    private async Task<IReadOnlyList<MastodonStatus>> GetStatuses() {

        string fileName = "TestStatuses.json";

        if (IsOffline) {
            try {
                return JsonUtils.LoadJsonArray(fileName, MastodonStatus.Parse);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error: Unable to read test statuses JSON file");
                return Array.Empty<MastodonStatus>();
            }
        }

        IReadOnlyList<MastodonStatus> statuses = await _mastodonService.GetStatuses(12);

        if (CreateOfflineFile) {
            try {
                JsonUtils.SaveJsonArray(fileName, statuses);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error: Unable to write test statuses JSON file");
            }
        }

        return statuses;

    }

}