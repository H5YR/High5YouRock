using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Constants
{
    public static class FeedItemLogSchemaConstants
    {
        public const string TableName = "FeedItemLog";

        public const string PrimaryKey = Id;

        public const string Id = nameof(FeedItemLog.Id);

        public const string ExternalId = nameof(FeedItemLog.ExternalId);

        public const string Source = nameof(FeedItemLog.Source);

        public const string CreatedAt = nameof(FeedItemLog.CreatedAt);
    }
}
