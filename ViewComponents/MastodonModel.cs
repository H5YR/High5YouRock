using Skybrud.Social.Mastodon.Models.Statuses;

namespace h5yr.ViewComponents;

public class MastodonModel {

    public IReadOnlyList<MastodonStatus> Statuses { get; }

    public MastodonModel(IReadOnlyList<MastodonStatus> statuses) {
        Statuses = statuses;
    }

}