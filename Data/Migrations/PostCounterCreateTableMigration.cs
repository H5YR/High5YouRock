using h5yr.Data.Constants;
using h5yr.Data.Entities;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;

namespace h5yr.Data.Migrations
{
    public class PostCounterCreateTableMigration : MigrationBase
    {
        private ILogger<PostCounterCreateTableMigration> _logger;
        private ICoreScopeProvider _scopeProvider;

        public PostCounterCreateTableMigration(IMigrationContext context, ILogger<PostCounterCreateTableMigration> logger, ICoreScopeProvider scopeProvider) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        protected override void Migrate()
        {
            _logger.LogDebug("Running migration.");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                if (!TableExists(PostCounterSchemaConstants.TableName))
                {
                    Create.Table<PostCounter>().Do();
                    scope.Complete();
                    return;
                }
                scope.Complete();
            }

            _logger.LogDebug(
                $"The database table {PostCounterSchemaConstants.TableName} already exists, skipping.");

        }
    }
}
