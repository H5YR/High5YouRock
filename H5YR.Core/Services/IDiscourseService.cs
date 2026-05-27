using H5YR.Core.Data.Entities;
using H5YR.Core.Models;
using H5YR.Core.Models.Discourse;

namespace H5YR.Core.Services
{
    public interface IDiscourseService
    {
        Task<bool> ProcessWebhookAsync(DiscourseWebhookPayload payload);
        IEnumerable<DiscourseReaction> GetRecentReactions(int count = 10);
        IEnumerable<UnifiedFeedItem> GetRecentReactionsAsFeedItems(int count = 10);
        int GetTotalReactionCount();
        string GetAvatarUrl(string avatarTemplate, int size = 120);
    }
}