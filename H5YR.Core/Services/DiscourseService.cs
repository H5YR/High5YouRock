using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using H5YR.Core.Models;
using H5YR.Core.Models.Discourse;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Services
{
    public class DiscourseService : IDiscourseService
    {
        private readonly ILogger<DiscourseService> _logger;
        private readonly IDiscourseReactionStore _reactionStore;
        private const string ForumBaseUrl = "https://forum.umbraco.com";

        public DiscourseService(
            ILogger<DiscourseService> logger, 
            IDiscourseReactionStore reactionStore)
        {
            _logger = logger;
            _reactionStore = reactionStore;
        }

        public async Task<bool> ProcessWebhookAsync(DiscourseWebhookPayload payload)
        {
            try
            {
                if (payload?.Like?.Post == null)
                {
                    _logger.LogWarning("Received invalid webhook payload");
                    return false;
                }

                var post = payload.Like.Post;
                var reactingUser = payload.Like.User;

                // Check if this is a "high-5-you-rock" reaction
                var hasHighFiveReaction = post.Reactions?
                    .Any(r => r.Id == "high-5-you-rock") ?? false;

                if (!hasHighFiveReaction)
                {
                    _logger.LogInformation("Post {PostId} does not have high-5-you-rock reaction, skipping", post.Id);
                    return false;
                }

                // Check if we already have this reaction
                if (_reactionStore.ReactionExists(post.Id))
                {
                    _logger.LogInformation("Reaction for post {PostId} already exists, skipping", post.Id);
                    return true;
                }

                var reactionInfo = post.Reactions.First(r => r.Id == "high-5-you-rock");

                // Create the entity
                var discourseReaction = new Data.Entities.DiscourseReaction
                {
                    PostId = post.Id,
                    PostUrl = ForumBaseUrl + post.PostUrl,
                    UserName = post.Username,
                    UserDisplayName = post.Name,
                    UserAvatar = GetAvatarUrl(post.AvatarTemplate),
                    PostContent = post.Raw,
                    CreatedAt = post.CreatedAt,
                    TopicTitle = post.TopicTitle,
                    TopicSlug = post.TopicSlug,
                    ReactionCount = reactionInfo.Count,
                    ReactingUserName = reactingUser?.Username ?? "Unknown",
                    ReceivedAt = DateTime.UtcNow
                };

                _reactionStore.Save(discourseReaction);
                _logger.LogInformation("Successfully saved Discourse reaction for post {PostId}", post.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Discourse webhook");
                return false;
            }
        }

        public IEnumerable<Data.Entities.DiscourseReaction> GetRecentReactions(int count = 10)
        {
            return _reactionStore.GetRecent(count);
        }

        public int GetTotalReactionCount()
        {
            return _reactionStore.GetTotalCount();
        }

        public string GetAvatarUrl(string avatarTemplate, int size = 120)
        {
            if (string.IsNullOrEmpty(avatarTemplate))
                return string.Empty;

            return ForumBaseUrl + avatarTemplate.Replace("{size}", size.ToString());
        }

        public IEnumerable<UnifiedFeedItem> GetRecentReactionsAsFeedItems(int count = 10)
        {
            var reactions = _reactionStore.GetRecent(count);
            
            return reactions.Select(r => new UnifiedFeedItem
            {
                Id = r.PostId.ToString(),
                SourceType = FeedSourceType.Discourse,
                CreatedAt = r.CreatedAt,
                UserName = r.UserName,
                UserDisplayName = r.UserDisplayName,
                UserAvatar = r.UserAvatar,
                Content = r.PostContent,
                RawContent = r.PostContent,
                Url = r.PostUrl,
                TopicTitle = r.TopicTitle,
                ReactionCount = r.ReactionCount
            });
        }
    }
}