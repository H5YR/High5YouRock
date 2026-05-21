using H5YR.Core.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;

namespace H5YR.Core.Composers
{
    public class FeedItemLogBackfillComposer : ComponentComposer<FeedItemLogBackfillComponent>, IComposer
    {
    }

    public class FeedItemLogBackfillComponent : IAsyncComponent
    {
        private const string BackfillDoneKey = "H5YR.FeedItemLogBackfill.Done";

        private readonly ILogger<FeedItemLogBackfillComponent> _logger;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly FeedItemLogBackfillService _backfillService;

        public FeedItemLogBackfillComponent(
            ILogger<FeedItemLogBackfillComponent> logger,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState,
            FeedItemLogBackfillService backfillService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValueService = keyValueService ?? throw new ArgumentNullException(nameof(keyValueService));
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            _backfillService = backfillService ?? throw new ArgumentNullException(nameof(backfillService));
        }

        public async Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            var alreadyDone = _keyValueService.GetValue(BackfillDoneKey);
            if (alreadyDone == "true")
            {
                _logger.LogDebug("FeedItemLog back-fill already completed, skipping.");
                return;
            }

            _logger.LogInformation("Running one-time FeedItemLog back-fill.");

            try
            {
                _backfillService.BackfillFromWidget();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Widget back-fill failed.");
            }

            try
            {
                await _backfillService.BackfillFromMastodonAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mastodon back-fill failed.");
            }

            _keyValueService.SetValue(BackfillDoneKey, "true");
            _logger.LogInformation("FeedItemLog back-fill complete. Flag set.");
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
