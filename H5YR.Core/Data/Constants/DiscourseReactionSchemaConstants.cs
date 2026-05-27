using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Constants
{
    public static class DiscourseReactionSchemaConstants
    {
        public const string TableName = "DiscourseReactions";
        public const string PrimaryKey = Id;

        public const string Id = nameof(DiscourseReaction.Id);
        public const string PostId = nameof(DiscourseReaction.PostId);
        public const string PostUrl = nameof(DiscourseReaction.PostUrl);
        public const string UserName = nameof(DiscourseReaction.UserName);
        public const string UserDisplayName = nameof(DiscourseReaction.UserDisplayName);
        public const string UserAvatar = nameof(DiscourseReaction.UserAvatar);
        public const string PostContent = nameof(DiscourseReaction.PostContent);
        public const string CreatedAt = nameof(DiscourseReaction.CreatedAt);
        public const string TopicTitle = nameof(DiscourseReaction.TopicTitle);
        public const string TopicSlug = nameof(DiscourseReaction.TopicSlug);
        public const string ReactionCount = nameof(DiscourseReaction.ReactionCount);
        public const string ReactingUserName = nameof(DiscourseReaction.ReactingUserName);
        public const string ReceivedAt = nameof(DiscourseReaction.ReceivedAt);
    }
}