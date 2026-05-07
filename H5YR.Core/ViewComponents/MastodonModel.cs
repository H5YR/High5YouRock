using Skybrud.Social.Mastodon.Models.Statuses;

namespace H5YR.Core.ViewComponents
{
   
    public class MastodonModel
    {

        public IReadOnlyList<MastodonStatus> Statuses { get; }

        public int TotalCount { get; }

        public MastodonModel(IReadOnlyList<MastodonStatus> statuses, int totalCount)
        {
            Statuses = statuses;
            TotalCount = totalCount;
        }

    }
}
