using h5yr.Data.Constants;
using h5yr.Data.Entities;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;

namespace h5yr.Data.Migrations
{
    public class TweetCounterCreateTableMigration : MigrationBase
    {
        private ILogger<TweetCounterCreateTableMigration> _logger;
        private ICoreScopeProvider _scopeProvider;

        public TweetCounterCreateTableMigration(IMigrationContext context, ILogger<TweetCounterCreateTableMigration> logger, ICoreScopeProvider scopeProvider) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        protected override void Migrate()
        {
            _logger.LogDebug("Running migration.");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                if (!TableExists(TweetCounterSchemaConstants.TableName))
                {
                    Create.Table<TweetCounter>().Do();
                    scope.Complete();
                    return;
                }
                scope.Complete();
            }

            _logger.LogDebug(
                $"The database table {TweetCounterSchemaConstants.TableName} already exists, skipping.");

        }
    }
}
