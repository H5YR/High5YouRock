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
    public class DiscourseReactionStoreComposer : ComponentComposer<DiscourseReactionStoreComponent>, IComposer
    {
    }

    public class DiscourseReactionStoreComponent : IAsyncComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger<DiscourseReactionStoreComponent> _logger;
        private readonly IRuntimeState _runtimeState;

        public DiscourseReactionStoreComponent(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor, 
            IKeyValueService keyValueService,
            ILogger<DiscourseReactionStoreComponent> logger, 
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
            _logger.LogInformation("DiscourseReactionStoreComponent.InitializeAsync called. RuntimeState.Level: {Level}", _runtimeState.Level);

            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                _logger.LogWarning("Runtime level is below Run, skipping migration");
                return Task.CompletedTask;
            }

            _logger.LogInformation("Creating migration plan for DiscourseReactions table");

            var migrationPlan = new MigrationPlan("DiscourseReactionCreateTableMigrationPlan");

            migrationPlan = migrationPlan.From(string.Empty)
                .To<DiscourseReactionCreateTableMigration>(nameof(DiscourseReactionCreateTableMigration));

            var upgrader = new Upgrader(migrationPlan);

            _logger.LogInformation("Executing DiscourseReaction migration plan");

            return upgrader.ExecuteAsync(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}