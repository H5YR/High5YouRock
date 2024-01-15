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
            // TODO - at the moment this is a hardcoded downloaded list based off umbracocommunity.social's API endpoint on 15/1/2024
            // (ie https://umbracocommunity.social/api/v1/custom_emojis ) and stored in the local MastodonCustomEmojis.json file.
            // Ideally this list would be called from the API directly to keep this always uptodate, however this would be better added in to
            // the Skybrud.Social.Mastodon package rather than the API calls being implemented here so it can benefit all

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            string fileName = "MastodonCustomEmojis.json";
            string jsonString = await System.IO.File.ReadAllTextAsync(fileName);
            return JsonSerializer.Deserialize<List<MastodonCustomEmoji>>(jsonString, options)!;
        }


    }

}
