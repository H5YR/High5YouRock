using H5YR.Core.Models;

namespace H5YR.Core.Services
{
    public interface IBlueskyService
    {
        IEnumerable<UnifiedFeedItem> GetPostsAsFeedItems(int limit);
        IEnumerable<FeedItem> GetFeedItemsBefore(DateTime before, int limit);
        Task<(List<UnifiedFeedItem> Items, string? NextCursor)> GetPostsPageAsync(int limit, string? cursor);
    }
}
