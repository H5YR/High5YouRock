using H5YR.Core.ViewComponents;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace H5YR.Core.Services
{
    public interface IMastodonService
    {
        Task<List<MastodonStatus>> GetStatuses(int limit, string? startId = null);
        Task<List<MastodonStatus>> GetStatusesPageAsync(int limit, string? maxId);
        int GetPostCount();
    }
}
