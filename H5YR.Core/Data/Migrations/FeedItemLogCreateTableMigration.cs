using H5YR.Core.Data.Constants;
using H5YR.Core.Data.Entities;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;

namespace H5YR.Core.Data.Migrations
{
    public class FeedItemLogCreateTableMigration : AsyncMigrationBase
    {
        private readonly ILogger<FeedItemLogCreateTableMigration> _logger;
        private readonly ICoreScopeProvider _scopeProvider;

        public FeedItemLogCreateTableMigration(IMigrationContext context, ILogger<FeedItemLogCreateTableMigration> logger, ICoreScopeProvider scopeProvider) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        protected override async Task MigrateAsync()
        {
            _logger.LogDebug("Running migration.");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                if (!TableExists(FeedItemLogSchemaConstants.TableName))
                {
                    Create.Table<FeedItemLog>().Do();
                    scope.Complete();
                    return;
                }
                scope.Complete();
            }

            _logger.LogDebug(
                $"The database table {FeedItemLogSchemaConstants.TableName} already exists, skipping.");
        }
    }
}
