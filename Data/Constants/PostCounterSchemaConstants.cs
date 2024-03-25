using h5yr.Data.Entities;

namespace h5yr.Data.Constants
{
    public static class PostCounterSchemaConstants
    {
        public const string TableName = "PostCounter";

        public const string PrimaryKey = Id;

        public const string Id = nameof(PostCounter.Id);

        public const string Date = nameof(PostCounter.Date);
        
        public const string Quantity = nameof(PostCounter.Quantity);
    }
}
