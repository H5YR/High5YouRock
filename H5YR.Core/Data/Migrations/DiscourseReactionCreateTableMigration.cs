using H5YR.Core.Data.Constants;
using H5YR.Core.Data.Entities;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;

namespace H5YR.Core.Data.Migrations
{
    public class DiscourseReactionCreateTableMigration : AsyncMigrationBase
    {
        private readonly ILogger<DiscourseReactionCreateTableMigration> _logger;
        private readonly ICoreScopeProvider _scopeProvider;

        public DiscourseReactionCreateTableMigration(
            IMigrationContext context, 
            ILogger<DiscourseReactionCreateTableMigration> logger, 
            ICoreScopeProvider scopeProvider) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        protected override async Task MigrateAsync()
        {
            _logger.LogDebug("Running DiscourseReaction migration.");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                if (!TableExists(DiscourseReactionSchemaConstants.TableName))
                {
                    _logger.LogInformation($"Creating table {DiscourseReactionSchemaConstants.TableName}");
                    Create.Table<DiscourseReaction>().Do();
                    scope.Complete();
                    _logger.LogInformation($"Table {DiscourseReactionSchemaConstants.TableName} created successfully");
                    return;
                }
                _logger.LogDebug($"The database table {DiscourseReactionSchemaConstants.TableName} already exists, skipping.");
                scope.Complete();
            }

            await Task.CompletedTask;
        }
    }
}