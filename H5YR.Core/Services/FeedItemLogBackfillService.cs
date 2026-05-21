using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Services
{
    public class FeedItemLogBackfillService
    {
        private readonly ILogger<FeedItemLogBackfillService> _logger;
        private readonly IFeedItemLogStore _feedItemLogStore;
        private readonly IWidgetH5yrStore _widgetH5yrStore;
        private readonly IMastodonService _mastodonService;

        private const int MastodonPageSize = 40;
        private static readonly TimeSpan BackfillWindow = TimeSpan.FromDays(90);

        public FeedItemLogBackfillService(
            ILogger<FeedItemLogBackfillService> logger,
            IFeedItemLogStore feedItemLogStore,
            IWidgetH5yrStore widgetH5yrStore,
            IMastodonService mastodonService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _feedItemLogStore = feedItemLogStore ?? throw new ArgumentNullException(nameof(feedItemLogStore));
            _widgetH5yrStore = widgetH5yrStore ?? throw new ArgumentNullException(nameof(widgetH5yrStore));
            _mastodonService = mastodonService ?? throw new ArgumentNullException(nameof(mastodonService));
        }

        public void BackfillFromWidget()
        {
            _logger.LogInformation("Starting FeedItemLog back-fill from WidgetH5yr.");

            var allWidgetItems = _widgetH5yrStore.GetAll();
            var toInsert = new List<FeedItemLog>();

            foreach (var w in allWidgetItems)
            {
                var externalId = $"widget_{w.Id}";
                if (!_feedItemLogStore.Exists(externalId))
                {
                    toInsert.Add(new FeedItemLog
                    {
                        ExternalId = externalId,
                        Source = "widget",
                        CreatedAt = w.CreatedAt
                    });
                }
            }

            if (toInsert.Count > 0)
            {
                _feedItemLogStore.SaveRange(toInsert);
                _logger.LogInformation("Back-filled {Count} widget items into FeedItemLog.", toInsert.Count);
            }
            else
            {
                _logger.LogInformation("No new widget items to back-fill.");
            }
        }

        public async Task BackfillFromMastodonAsync()
        {
            _logger.LogInformation("Starting FeedItemLog back-fill from Mastodon (window: {Days} days).", BackfillWindow.Days);

            var cutoff = DateTime.UtcNow - BackfillWindow;
            string? maxId = null;
            int totalInserted = 0;
            int pageCount = 0;

            while (true)
            {
                var page = await _mastodonService.GetStatusesPageAsync(MastodonPageSize, maxId);

                if (page.Count == 0)
                {
                    _logger.LogInformation("Mastodon back-fill reached end of timeline after {Pages} pages.", pageCount);
                    break;
                }

                pageCount++;
                var toInsert = new List<FeedItemLog>();

                foreach (var status in page)
                {
                    var createdAt = status.CreatedAt.DateTimeOffset.UtcDateTime;

                    if (createdAt < cutoff)
                    {
                        _logger.LogInformation("Mastodon back-fill reached cut-off date after {Pages} pages.", pageCount);
                        goto Done;
                    }

                    var externalId = $"mastodon_{status.Id}";
                    if (!_feedItemLogStore.Exists(externalId))
                    {
                        toInsert.Add(new FeedItemLog
                        {
                            ExternalId = externalId,
                            Source = "mastodon",
                            CreatedAt = createdAt
                        });
                    }
                }

                if (toInsert.Count > 0)
                {
                    _feedItemLogStore.SaveRange(toInsert);
                    totalInserted += toInsert.Count;
                }

                // Use the ID of the oldest status in the page as the cursor for the next page
                maxId = page.Last().Id;

                // Polite pause to avoid hammering the API
                await Task.Delay(500);
            }

            Done:
            _logger.LogInformation("Mastodon back-fill complete. Inserted {Count} items across {Pages} pages.", totalInserted, pageCount);
        }
    }
}
