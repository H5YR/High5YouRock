using h5yr.ViewComponents;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.Core.Services
{
    public interface IMastodonService
    {
        Task<List<MastodonStatus>> GetStatuses(int limit, string? startId = null);

        Task<List<MastodonCustomEmoji>> GetCustomEmojis();

    }
}
