using System.Text.Json.Serialization;

namespace H5YR.Core.Models.Discourse
{
    public class DiscourseWebhookPayload
    {
        [JsonPropertyName("like")]
        public DiscourseWebhookLike? Like { get; set; }
    }

    public class DiscourseWebhookLike
    {
        [JsonPropertyName("post")]
        public DiscoursePost? Post { get; set; }

        [JsonPropertyName("user")]
        public DiscourseUser? User { get; set; }
    }

    public class DiscoursePost
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("avatar_template")]
        public string AvatarTemplate { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("cooked")]
        public string Cooked { get; set; } = string.Empty;

        [JsonPropertyName("raw")]
        public string Raw { get; set; } = string.Empty;

        [JsonPropertyName("post_url")]
        public string PostUrl { get; set; } = string.Empty;

        [JsonPropertyName("topic_title")]
        public string TopicTitle { get; set; } = string.Empty;

        [JsonPropertyName("topic_slug")]
        public string TopicSlug { get; set; } = string.Empty;

        [JsonPropertyName("reactions")]
        public List<DiscourseReactionInfo>? Reactions { get; set; }
    }

    public class DiscourseReactionInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class DiscourseUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("avatar_template")]
        public string AvatarTemplate { get; set; } = string.Empty;
    }
}