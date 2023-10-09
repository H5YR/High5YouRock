using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Models.Statuses;
using Skybrud.Social.Mastodon.Options.Timeline;
using Skybrud.Social.Mastodon.Responses.Statuses;

namespace h5yr.Core.Services {

    public class MastodonService : IMastodonService
    {

        private readonly ILogger<MastodonService> _logger;

        public MastodonService(ILogger<MastodonService> logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<MastodonStatus> GetStatuses(int limit, string? maxId = null)
        {

            // Initialize a new HTTP service (basically the API wrapper)
            MastodonHttpService mastodon = MastodonHttpService
                .CreateFromDomain("umbracocommunity.social");

            // Initialize the options for the request to the API
            MastodonGetHashtagTimelineOptions options = new()
            {
                Hashtag = "h5yr",
                Limit = limit
            };

            try
            {

                // Make the request to the API
                MastodonStatusListResponse response = mastodon
                    .Timelines
                    .GetHashtagTimeline(options);

                // Return the statuses
                return response.Body;

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed fetching statuses from the Mastodon API.");

            }


            return Array.Empty<MastodonStatus>();

        }

    }

}
