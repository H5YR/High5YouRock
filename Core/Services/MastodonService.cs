using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Models.Statuses;
using Skybrud.Social.Mastodon.Options.Timeline;
using Skybrud.Social.Mastodon.Responses.Statuses;
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

    }

}
