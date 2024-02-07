using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.ViewComponents;

public class MastodonModel {

    public IReadOnlyList<MastodonStatus> Statuses { get; }

    public IReadOnlyList<MastodonCustomEmoji> CustomEmojis { get; }

    public MastodonModel(IReadOnlyList<MastodonStatus> statuses, IReadOnlyList<MastodonCustomEmoji> customEmojis)
    {
        Statuses = statuses;
        CustomEmojis = customEmojis;
    }

}