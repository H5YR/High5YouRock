using H5YR.Core.Models;
using H5YR.Core.ViewComponents;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace H5YR.Core.Services
{
    public interface IMastodonService
    {
        Task<List<MastodonStatus>> GetStatuses(int limit, string? startId = null);
        int GetPostCount();
        IEnumerable<UnifiedFeedItem> GetStatusesAsFeedItems(int limit);
    }
}
