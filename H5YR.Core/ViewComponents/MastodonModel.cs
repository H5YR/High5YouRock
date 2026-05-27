using H5YR.Core.Models;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace H5YR.Core.ViewComponents
{

    public class MastodonModel
    {

        public IReadOnlyList<MastodonStatus> Statuses { get; }

        /// <summary>
        /// Unified feed items combining Mastodon posts and widget h5yr submissions, sorted by date descending.
        /// </summary>
        public IReadOnlyList<FeedItem> FeedItems { get; }

        public int TotalCount { get; }

        public MastodonModel(IReadOnlyList<MastodonStatus> statuses, int totalCount, IReadOnlyList<FeedItem>? feedItems = null)
        {
            Statuses = statuses;
            TotalCount = totalCount;
            FeedItems = feedItems ?? Array.Empty<FeedItem>();
        }

    }
}
