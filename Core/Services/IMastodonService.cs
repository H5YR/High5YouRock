using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.Core.Services
{
    public interface IMastodonService
    {
        Task<List<MastodonStatus>> GetStatuses(int limit);
    }
}
