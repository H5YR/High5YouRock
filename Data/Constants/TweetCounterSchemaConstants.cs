using h5yr.Data.Entities;

namespace h5yr.Data.Constants
{
    public static class TweetCounterSchemaConstants
    {
        public const string TableName = "TweetCounter";

        public const string PrimaryKey = Id;

        public const string Id = nameof(TweetCounter.Id);

        public const string Date = nameof(TweetCounter.Date);
        
        public const string Quantity = nameof(TweetCounter.Quantity);
    }
}
