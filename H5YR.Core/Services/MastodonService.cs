using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using H5YR.Core.Models;
using H5YR.Core.ViewComponents;
using Microsoft.Extensions.Logging;
using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Models.Statuses;
using Skybrud.Social.Mastodon.Options.Timeline;
using Skybrud.Social.Mastodon.Responses.Statuses;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;

namespace H5YR.Core.Services
{

    public class MastodonService : IMastodonService
    {

        private readonly ILogger<MastodonService> _logger;
        private readonly AppCaches _appCaches;
        private readonly IPostCounterStore _postCounterStore;
        private const string FeedDomain = "umbracocommunity.social";
        private const string FeedHashtag = "h5yr";
        private const string FeedCacheKey = "mastodonposts";
        private const int FeedCacheMinutes = 15;


        public MastodonService(ILogger<MastodonService> logger, AppCaches appCaches, IPostCounterStore postCounterStore)
        {
            _logger = logger;
            _appCaches = appCaches;
            _postCounterStore = postCounterStore;
        }

        public async Task<List<MastodonStatus>> GetStatuses(int limit, string? startId = null)
        {
            var posts = _appCaches.RuntimeCache.GetCacheItem($"{FeedCacheKey}_{limit}_{startId}",
                () => LoadStatuses(limit, startId),
                TimeSpan.FromMinutes(FeedCacheMinutes));

            return await posts!;
        }

        public async Task<List<MastodonStatus>> GetStatusesPageAsync(int limit, string? maxId)
        {
            MastodonHttpService mastodon = MastodonHttpService.CreateFromDomain(FeedDomain);

            MastodonGetHashtagTimelineOptions options = new()
            {
                Hashtag = FeedHashtag,
                Limit = limit,
                MaxId = maxId
            };

            try
            {
                MastodonStatusListResponse response = await mastodon.Timelines.GetHashtagTimelineAsync(options);
                return response.Body.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed fetching paginated statuses from the Mastodon API (maxId={MaxId}).", maxId);
                return new List<MastodonStatus>();
            }
        }


        private async Task<List<MastodonStatus>> LoadStatuses(int limit, string? startId)
        {

            // Initialize a new HTTP service (basically the API wrapper)
            MastodonHttpService mastodon = MastodonHttpService
                .CreateFromDomain(FeedDomain);

            // Initialize the options for the request to the API
            MastodonGetHashtagTimelineOptions options = new()
            {
                Hashtag = FeedHashtag,
                Limit = limit,
                MaxId = startId
            };

            List<MastodonStatus> posts = new List<MastodonStatus>();

            try
            {
                // Make the request to the API
                MastodonStatusListResponse response = await mastodon
                    .Timelines
                    .GetHashtagTimelineAsync(options);

                // Return the statuses
                posts = response.Body.ToList();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed fetching statuses from the Mastodon API.");
            }

            try
            {
                // Update our post count table
                UpdatePostCount(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed updating local post count after updating from Mastodon API.");
            }


            return posts;

        }

        public int GetPostCount()
        {
            return _postCounterStore.GetPostCount();
        }

        private void UpdatePostCount(List<MastodonStatus> latestPosts)
        {
            // Check the latest batch of posts just loaded from the API. If any of these are newer than add a new row with the latest timestamp to the local count table
            var postCounter = _postCounterStore.GetAll().LastOrDefault();

            if (postCounter == null)
            {
                // Default first seed for new DB - manually counted 126 Mastodon posts from July 2023 - 21/3/2024
                // Ideally would have done this during initial migration, but it inserts a corrupt date if done here.
                postCounter = new PostCounter()
                {
                    Date = DateTime.Parse("2024-03-22 11:00:00"),
                    Quantity = 126
                };
                _postCounterStore.Save(postCounter);
            }

            var newerPosts = latestPosts.Where(p => p.CreatedAt.DateTimeOffset.DateTime > postCounter.Date.AddMilliseconds(3)); // 3ms covers for SQL datetime rounding
            if (newerPosts.Any())
            {
                // We have newer posts, so update the count
                var postCountModel = new PostCounter()
                {
                    Date = newerPosts.First().CreatedAt.DateTimeOffset.DateTime,
                    Quantity = postCounter.Quantity += newerPosts.Count()
                };
                _postCounterStore.Update(postCountModel);
            }
        }

        public IEnumerable<UnifiedFeedItem> GetStatusesAsFeedItems(int limit)
        {
            var statuses = GetStatuses(limit).GetAwaiter().GetResult();
            
            return statuses.Select(s => new UnifiedFeedItem
            {
                Id = s.Id,
                SourceType = FeedSourceType.Mastodon,
                CreatedAt = s.CreatedAt.DateTimeOffset.DateTime,
                UserName = s.Account.Username,
                UserDisplayName = s.Account.DisplayName,
                UserAvatar = s.Account.Avatar,
                Content = s.Content,
                RawContent = s.Content,
                Url = s.Url
            });
        }

    }

}
