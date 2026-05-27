using H5YR.Core.Data.Entities;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;

namespace H5YR.Core.Data.Migrations
{
    public class DiscourseReactionCreateTableMigration : MigrationBase
    {
        public DiscourseReactionCreateTableMigration(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            if (!TableExists("DiscourseReactions"))
            {
                Create.Table<DiscourseReaction>().Do();
            }
        }
    }
}