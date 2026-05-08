namespace H5YR.Core.Models
{
    /// <summary>
    /// Unified feed item that represents either a Mastodon post or a widget h5yr submission.
    /// Used for rendering both sources in the same feed grid.
    /// </summary>
    public class FeedItem
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// "mastodon" or "widget"
        /// </summary>
        public string Source { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string ProfileUrl { get; set; } = string.Empty;

        /// <summary>
        /// HTML content to render. For Mastodon posts this includes custom emojis.
        /// For widget posts this is the auto-generated "#h5yr for {url}" message.
        /// </summary>
        public string ContentHtml { get; set; } = string.Empty;

        /// <summary>
        /// Permalink to the original post. For Mastodon this is the toot URL.
        /// For widget posts this is the target page URL.
        /// </summary>
        public string Permalink { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The auth provider badge for widget posts (e.g. "github", "google").
        /// Null for Mastodon posts.
        /// </summary>
        public string? AuthProvider { get; set; }
    }
}
