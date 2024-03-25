using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.ViewComponents;

public class MastodonModel {

    public IReadOnlyList<MastodonStatus> Statuses { get; }

    public IReadOnlyList<MastodonCustomEmoji> CustomEmojis { get; }

    public int TotalCount { get; }

    public MastodonModel(IReadOnlyList<MastodonStatus> statuses, IReadOnlyList<MastodonCustomEmoji> customEmojis, int totalCount)
    {
        Statuses = statuses;
        CustomEmojis = customEmojis;
        TotalCount = totalCount;
    }

}