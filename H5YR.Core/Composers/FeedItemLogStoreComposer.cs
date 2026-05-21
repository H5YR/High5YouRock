using H5YR.Core.Data.Migrations;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace H5YR.Core.Composers
{
    public class FeedItemLogStoreComposer : ComponentComposer<FeedItemLogStoreComponent>, IComposer
    {
    }

    public class FeedItemLogStoreComponent : IAsyncComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger<FeedItemLogStoreComponent> _logger;
        private readonly IRuntimeState _runtimeState;

        public FeedItemLogStoreComponent(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            ILogger<FeedItemLogStoreComponent> logger,
            IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider ?? throw new ArgumentNullException(nameof(coreScopeProvider));
            _migrationPlanExecutor = migrationPlanExecutor ?? throw new ArgumentNullException(nameof(migrationPlanExecutor));
            _keyValueService = keyValueService ?? throw new ArgumentNullException(nameof(keyValueService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
        }

        public Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
            }

            var migrationPlan = new MigrationPlan("FeedItemLogCreateTableMigrationPlan1");

            migrationPlan = migrationPlan.From(string.Empty)
                .To<FeedItemLogCreateTableMigration>(nameof(FeedItemLogCreateTableMigration));

            var upgrader = new Upgrader(migrationPlan);

            return upgrader.ExecuteAsync(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
