using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.Core.Services
{
    public interface IMastodonService
    {
        Task<IReadOnlyList<MastodonStatus>> GetStatuses(int limit, string? maxId = null);
    }
}
