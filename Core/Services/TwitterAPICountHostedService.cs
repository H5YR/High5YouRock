using System;
using System.Threading.Tasks;
using h5yr.Data.Entities;
using h5yr.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace H5YR.Core.Services
{
    public class TwitterAPICountHostedService : RecurringHostedServiceBase
    {
        private readonly IRuntimeState _runtimeState;
        private readonly IContentService _contentService;
        private readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly IProfilingLogger _profilingLogger;
        private readonly ILogger<TwitterAPICountHostedService> _logger;
        private readonly ICoreScopeProvider _scopeProvider;
        private readonly ITwitterHelper _twitterHelper;
        private readonly ITweetCounterStore _tweetCounterStore;

        private static TimeSpan HowOftenWeRepeat => TimeSpan.FromHours(6);
        private static TimeSpan DelayBeforeWeStart => TimeSpan.FromMinutes(1);

        public TwitterAPICountHostedService(
            IRuntimeState runtimeState,
            IContentService contentService,
            IServerRoleAccessor serverRoleAccessor,
            IProfilingLogger profilingLogger,
            ILogger<TwitterAPICountHostedService> logger,
            ICoreScopeProvider scopeProvider,
            ITwitterHelper twitterHelper,
            ITweetCounterStore tweetCounterStore)
            : base(logger, HowOftenWeRepeat, DelayBeforeWeStart)
        {
            _runtimeState = runtimeState;
            _contentService = contentService;
            _serverRoleAccessor = serverRoleAccessor;
            _profilingLogger = profilingLogger;
            _logger = logger;
            _scopeProvider = scopeProvider;
            _twitterHelper = twitterHelper;
            _tweetCounterStore = tweetCounterStore;
        }

        public override Task PerformExecuteAsync(object state)
        {
            // Don't do anything if the site is not running.
            if (_runtimeState.Level != RuntimeLevel.Run)
            {
                return Task.CompletedTask;
            }

            // Wrap the three content service calls in a scope to do it all in one transaction.
            using ICoreScope scope = _scopeProvider.CreateCoreScope();

            _logger.LogInformation("Retrieving tweet count for {0}", DateTime.Now.ToString("dd/MM/yyyy"));
            var tweetCount = _twitterHelper.GetTweetCount();
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            var tweetCountModel = new TweetCounter()
            {
                Date = date,
                Quantity = tweetCount,
            };

            _tweetCounterStore.Update(tweetCountModel);

            // Remember to complete the scope when done.
            scope.Complete();
            return Task.CompletedTask;
        }
    }
}
