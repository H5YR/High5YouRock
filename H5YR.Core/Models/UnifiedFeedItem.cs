namespace H5YR.Core.Models
{
    public enum FeedSourceType
    {
        Mastodon,
        Discourse,
        Widget
    }

    public class UnifiedFeedItem
    {
        public string Id { get; set; } = string.Empty;
        public FeedSourceType SourceType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string RawContent { get; set; } = string.Empty;

        // Discourse-specific
        public string? TopicTitle { get; set; }
        public int? ReactionCount { get; set; }

        // For ordering
        public long SortTimestamp => CreatedAt.Ticks;
    }
}