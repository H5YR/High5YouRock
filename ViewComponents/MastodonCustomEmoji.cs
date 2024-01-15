using Skybrud.Social.Mastodon.Models.Statuses;
using System.Text.Json.Serialization;

namespace h5yr.ViewComponents;

public class MastodonCustomEmoji {

    /// <summary>
    /// Based off json model from:- https://docs.joinmastodon.org/methods/custom_emojis/
    /// </summary>
    public string? Shortcode { get; set; }

    public string? Url { get; set; }

    [JsonPropertyName("static_url")]

    public string? StaticUrl { get; set; }

    [JsonPropertyName("visible_in_picker")]
    public bool? VisibleInPicker { get; set; }

    public string? Category { get; set; }

}