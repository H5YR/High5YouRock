using H5YR.Core.Data.Constants;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace H5YR.Core.Data.Entities
{
    [TableName(FeedItemLogSchemaConstants.TableName)]
    [PrimaryKey(FeedItemLogSchemaConstants.PrimaryKey, AutoIncrement = true)]
    [ExplicitColumns]
    public class FeedItemLog
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column(FeedItemLogSchemaConstants.Id)]
        public int Id { get; set; }

        /// <summary>
        /// The original ID of the feed item (e.g. Mastodon status ID or widget row ID).
        /// Used to prevent duplicate inserts.
        /// </summary>
        [Column(FeedItemLogSchemaConstants.ExternalId)]
        [Length(255)]
        public string ExternalId { get; set; } = string.Empty;

        /// <summary>
        /// "mastodon" or "widget"
        /// </summary>
        [Column(FeedItemLogSchemaConstants.Source)]
        [Length(50)]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// The original timestamp of the feed item, used for date-based stats.
        /// </summary>
        [Column(FeedItemLogSchemaConstants.CreatedAt)]
        public DateTime CreatedAt { get; set; }
    }
}
