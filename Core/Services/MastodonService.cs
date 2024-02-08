using h5yr.ViewComponents;
using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Models.Statuses;
using Skybrud.Social.Mastodon.Options.Timeline;
using Skybrud.Social.Mastodon.Responses.Statuses;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;

namespace h5yr.Core.Services {

    public class MastodonService : IMastodonService
    {

        private readonly ILogger<MastodonService> _logger;
        private readonly AppCaches _appCaches;

        private const string FeedDomain = "umbracocommunity.social";
        private const string FeedHashtag = "h5yr";
        private const string FeedCacheKey = "mastodonposts";
        private const int FeedCacheMinutes = 15;
        private const string EmojiCacheKey = "mastodonemojis";



        public MastodonService(ILogger<MastodonService> logger, AppCaches appCaches)
        {
            _logger = logger;
            _appCaches = appCaches;
        }

        public async Task<List<MastodonStatus>> GetStatuses(int limit)
        {
            var posts = _appCaches.RuntimeCache.GetCacheItem($"{FeedCacheKey}_{limit}",
                () => LoadStatuses(limit),
                TimeSpan.FromMinutes(FeedCacheMinutes));

            return await posts!;

            if (posts != null)
            {
                return await posts;
            }
            else
            {
                return await Task.FromResult(new List<MastodonStatus>());
            }

        }


        private async Task<List<MastodonStatus>> LoadStatuses(int limit)
        {

            // Initialize a new HTTP service (basically the API wrapper)
            MastodonHttpService mastodon = MastodonHttpService
                .CreateFromDomain(FeedDomain);

            // Initialize the options for the request to the API
            MastodonGetHashtagTimelineOptions options = new()
            {
                Hashtag = FeedHashtag,
                Limit = limit
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

        public async Task<List<MastodonCustomEmoji>> GetCustomEmojis()
        {
            var emojis = _appCaches.RuntimeCache.GetCacheItem($"{EmojiCacheKey}",
                () => LoadCustomEmojis());

            return await emojis!;
        }

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
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true} );
                }            
            }
            catch
            {
                customEmojis = new List<MastodonCustomEmoji>();
            }
            return customEmojis!;
        }


    }

}
