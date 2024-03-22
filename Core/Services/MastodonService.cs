using h5yr.Data.Entities;
using h5yr.Data.Interfaces;
using h5yr.ViewComponents;
using Lucene.Net.Documents;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Models.Statuses;
using Skybrud.Social.Mastodon.Options.Timeline;
using Skybrud.Social.Mastodon.Responses.Statuses;
using System.Text.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Scoping;

namespace h5yr.Core.Services {

    public class MastodonService : IMastodonService
    {

        private readonly ILogger<MastodonService> _logger;
        private readonly AppCaches _appCaches;
        private readonly ICoreScopeProvider _scopeProvider;
        private readonly IPostCounterStore _postCounterStore;

        private const string FeedDomain = "umbracocommunity.social";
        private const string FeedHashtag = "h5yr";
        private const string FeedCacheKey = "mastodonposts";
        private const int FeedCacheMinutes = 15;
        private const string EmojiCacheKey = "mastodonemojis";



        public MastodonService(ILogger<MastodonService> logger, AppCaches appCaches, ICoreScopeProvider scopeProvider, IPostCounterStore postCounterStore)
        {
            _logger = logger;
            _appCaches = appCaches;
            _scopeProvider = scopeProvider;
            _postCounterStore = postCounterStore;
        }

        public async Task<List<MastodonStatus>> GetStatuses(int limit, string? startId = null)
        {
            var posts = _appCaches.RuntimeCache.GetCacheItem($"{FeedCacheKey}_{limit}_{startId}",
                () => LoadStatuses(limit, startId),
                TimeSpan.FromMinutes(FeedCacheMinutes));

            return await posts!;
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

            try
            {
                // Make the request to the API
                MastodonStatusListResponse response = await mastodon
                    .Timelines
                    .GetHashtagTimelineAsync(options);

                // Return the statuses
                return response.Body.ToList();


            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed fetching statuses from the Mastodon API.");

            }


            return new List<MastodonStatus>();

        }

        [Obsolete("This will be removed and use Custom Emojis from the Skybrud package")]
        public async Task<List<MastodonCustomEmoji>> GetCustomEmojis()
        {
            var emojis = _appCaches.RuntimeCache.GetCacheItem($"{EmojiCacheKey}",
                () => LoadCustomEmojis());

            return await emojis!;
        }

        [Obsolete("This will be removed and use Custom Emojis from the Skybrud package")]
        private async Task<List<MastodonCustomEmoji>> LoadCustomEmojis()
        {
            // TODO - at the moment this is calling the Mastodon API endpoint directly for the list of custom emojis.
            // (ie https://umbracocommunity.social/api/v1/custom_emojis )
            // Ideally this would be better added in to the Skybrud.Social.Mastodon package later
            // to keep all API calls in the external library and so it can benefit all users of the nuget package

            var customEmojis = new List<MastodonCustomEmoji>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var endpoint = "https://umbracocommunity.social/api/v1/custom_emojis";
                    customEmojis = await client.GetFromJsonAsync<List<MastodonCustomEmoji>>(endpoint,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
            }
            catch
            {
                customEmojis = new List<MastodonCustomEmoji>();
            }
            return customEmojis!;
        }

        private void UpdatePostCount()
        {
            // TODO - complete Mastodon implementation
            // Wrap the three content service calls in a scope to do it all in one transaction.
            using ICoreScope scope = _scopeProvider.CreateCoreScope();

            var postCount = 15; // Do counting from above
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            _logger.LogInformation("Retrieving post count for " + DateTime.Now.ToString("dd/MM/yyyy") + ". " + postCount + " posts found");

            var postCountModel = new PostCounter()
            {
                Date = date,
                Quantity = postCount,
            };

            _postCounterStore.Update(postCountModel);

            // Remember to complete the scope when done.
            scope.Complete();
        }


    }

}
