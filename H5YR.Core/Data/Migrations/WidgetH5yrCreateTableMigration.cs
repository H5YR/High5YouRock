using H5YR.Core.Data.Constants;
using H5YR.Core.Data.Entities;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;

namespace H5YR.Core.Data.Migrations
{
    public class WidgetH5yrCreateTableMigration : AsyncMigrationBase
    {
        private readonly ILogger<WidgetH5yrCreateTableMigration> _logger;
        private readonly ICoreScopeProvider _scopeProvider;

        public WidgetH5yrCreateTableMigration(
            IMigrationContext context,
            ILogger<WidgetH5yrCreateTableMigration> logger,
            ICoreScopeProvider scopeProvider) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        protected override async Task MigrateAsync()
        {
            _logger.LogDebug("Running WidgetH5yr table migration.");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                if (!TableExists(WidgetH5yrSchemaConstants.TableName))
                {
                    Create.Table<WidgetH5yr>().Do();
                    scope.Complete();
                    return;
                }
                scope.Complete();
            }

            _logger.LogDebug(
                $"The database table {WidgetH5yrSchemaConstants.TableName} already exists, skipping.");
        }
    }
}
