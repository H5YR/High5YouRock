using H5YR.Core.Models;
using H5YR.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;

namespace H5YR.Core.Services
{
    public class BlueskyService : IBlueskyService
    {
        private readonly ILogger<BlueskyService> _logger;
        private readonly AppCaches _appCaches;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<BlueskySettings> _settings;

        private const string FeedCacheKey = "blueskyposts";
        private const string SessionCacheKey = "blueskysession";
        private const int FeedCacheMinutes = 15;
        private const int SessionCacheMinutes = 60;
        private const int MaxPageSize = 100;

        public BlueskyService(
            ILogger<BlueskyService> logger,
            AppCaches appCaches,
            IHttpClientFactory httpClientFactory,
            IOptions<BlueskySettings> settings)
        {
            _logger = logger;
            _appCaches = appCaches;
            _httpClientFactory = httpClientFactory;
            _settings = settings;
        }

        private bool IsConfigured =>
            !string.IsNullOrWhiteSpace(_settings.Value.Identifier) &&
            !string.IsNullOrWhiteSpace(_settings.Value.AppPassword);

        public IEnumerable<UnifiedFeedItem> GetPostsAsFeedItems(int limit)
        {
            var posts = GetCachedPosts().GetAwaiter().GetResult();
            return posts.Take(limit);
        }

        public IEnumerable<FeedItem> GetFeedItemsBefore(DateTime before, int limit)
        {
            var posts = GetCachedPosts().GetAwaiter().GetResult();

            return posts
                .Where(p => p.CreatedAt < before)
                .Take(limit)
                .Select(p => new FeedItem
                {
                    Id = $"bluesky_{p.Id}",
                    Source = "bluesky",
                    AvatarUrl = p.UserAvatar,
                    Username = p.UserName,
                    ProfileUrl = $"https://bsky.app/profile/{p.UserName}",
                    ContentHtml = p.Content,
                    Permalink = p.Url,
                    CreatedAt = p.CreatedAt
                })
                .ToList();
        }

        public async Task<(List<UnifiedFeedItem> Items, string? NextCursor)> GetPostsPageAsync(int limit, string? cursor)
        {
            if (!IsConfigured)
            {
                return (new List<UnifiedFeedItem>(), null);
            }

            try
            {
                return await SearchAsync(limit, cursor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed fetching paginated posts from the Bluesky API.");
                return (new List<UnifiedFeedItem>(), null);
            }
        }

        private async Task<List<UnifiedFeedItem>> GetCachedPosts()
        {
            var posts = _appCaches.RuntimeCache.GetCacheItem(FeedCacheKey,
                LoadPosts,
                TimeSpan.FromMinutes(FeedCacheMinutes));

            return await posts!;
        }

        private async Task<List<UnifiedFeedItem>> LoadPosts()
        {
            if (!IsConfigured)
            {
                _logger.LogDebug("Bluesky credentials not configured, skipping Bluesky feed.");
                return new List<UnifiedFeedItem>();
            }

            try
            {
                var (items, _) = await SearchAsync(MaxPageSize, null);
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed fetching posts from the Bluesky API.");
                return new List<UnifiedFeedItem>();
            }
        }

        private async Task<(List<UnifiedFeedItem> Items, string? NextCursor)> SearchAsync(int limit, string? cursor)
        {
            var settings = _settings.Value;
            var token = await GetAccessTokenAsync();

            var query = $"q={Uri.EscapeDataString("#" + settings.Hashtag)}&tag={Uri.EscapeDataString(settings.Hashtag)}&sort=latest&limit={Math.Min(limit, MaxPageSize)}";
            if (!string.IsNullOrEmpty(cursor))
            {
                query += $"&cursor={Uri.EscapeDataString(cursor)}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{settings.ServiceUrl.TrimEnd('/')}/xrpc/app.bsky.feed.searchPosts?{query}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var client = _httpClientFactory.CreateClient();
            using var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Token may have expired - drop it so the next call creates a fresh session
                _appCaches.RuntimeCache.Clear(SessionCacheKey);
            }

            response.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = doc.RootElement;

            var items = new List<UnifiedFeedItem>();
            if (root.TryGetProperty("posts", out var posts))
            {
                foreach (var post in posts.EnumerateArray())
                {
                    try
                    {
                        var item = MapPost(post);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        // One malformed post must not take down the whole feed
                        _logger.LogWarning(ex, "Skipping malformed Bluesky post.");
                    }
                }
            }

            var next = root.TryGetProperty("cursor", out var c) ? c.GetString() : null;
            return (items.OrderByDescending(i => i.CreatedAt).ToList(), next);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var cached = _appCaches.RuntimeCache.GetCacheItem<string>(SessionCacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                return cached;
            }

            var settings = _settings.Value;
            var client = _httpClientFactory.CreateClient();
            using var response = await client.PostAsJsonAsync(
                $"{settings.ServiceUrl.TrimEnd('/')}/xrpc/com.atproto.server.createSession",
                new { identifier = settings.Identifier, password = settings.AppPassword });
            response.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var token = doc.RootElement.GetProperty("accessJwt").GetString()
                ?? throw new InvalidOperationException("Bluesky session response did not contain an access token.");

            _appCaches.RuntimeCache.InsertCacheItem(SessionCacheKey, () => token, TimeSpan.FromMinutes(SessionCacheMinutes));
            return token;
        }

        private static UnifiedFeedItem? MapPost(JsonElement post)
        {
            if (!post.TryGetProperty("uri", out var uriEl) ||
                !post.TryGetProperty("author", out var author) ||
                !post.TryGetProperty("record", out var record))
            {
                return null;
            }

            var uri = uriEl.GetString();
            var handle = author.TryGetProperty("handle", out var h) ? h.GetString() : null;
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(handle))
            {
                return null;
            }

            var text = record.TryGetProperty("text", out var t) ? t.GetString() ?? "" : "";

            DateTimeOffset createdAt;
            if (!(record.TryGetProperty("createdAt", out var ca) && DateTimeOffset.TryParse(ca.GetString(), out createdAt)) &&
                !(post.TryGetProperty("indexedAt", out var ia) && DateTimeOffset.TryParse(ia.GetString(), out createdAt)))
            {
                return null;
            }

            // Guard against client-supplied future timestamps pinning a post to the top of the feed
            var created = createdAt.UtcDateTime;
            if (created > DateTime.UtcNow)
            {
                created = DateTime.UtcNow;
            }

            var rkey = uri.Substring(uri.LastIndexOf('/') + 1);
            var displayName = author.TryGetProperty("displayName", out var dn) ? dn.GetString() : null;
            var avatar = author.TryGetProperty("avatar", out var av) ? av.GetString() : null;
            var html = RenderHtml(text, record.TryGetProperty("facets", out var f) ? f : (JsonElement?)null);

            return new UnifiedFeedItem
            {
                Id = uri,
                SourceType = FeedSourceType.Bluesky,
                CreatedAt = created,
                UserName = handle,
                UserDisplayName = string.IsNullOrWhiteSpace(displayName) ? handle : displayName,
                UserAvatar = avatar ?? string.Empty,
                Content = html,
                RawContent = text,
                Url = $"https://bsky.app/profile/{handle}/post/{rkey}"
            };
        }

        /// <summary>
        /// Converts post text to safe HTML, linking facets. Facet indices are UTF-8 byte offsets.
        /// </summary>
        private static string RenderHtml(string text, JsonElement? facets)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var links = new List<(int Start, int End, string Url)>();

            if (facets is { ValueKind: JsonValueKind.Array })
            {
                foreach (var facet in facets.Value.EnumerateArray())
                {
                    if (!facet.TryGetProperty("index", out var index) ||
                        !facet.TryGetProperty("features", out var features) ||
                        !index.TryGetProperty("byteStart", out var bs) ||
                        !index.TryGetProperty("byteEnd", out var be))
                    {
                        continue;
                    }

                    string? url = null;
                    foreach (var feature in features.EnumerateArray())
                    {
                        var type = feature.TryGetProperty("$type", out var ty) ? ty.GetString() : null;
                        if (type == "app.bsky.richtext.facet#link" && feature.TryGetProperty("uri", out var u))
                        {
                            url = u.GetString();
                        }
                        else if (type == "app.bsky.richtext.facet#tag" && feature.TryGetProperty("tag", out var tag))
                        {
                            url = $"https://bsky.app/hashtag/{Uri.EscapeDataString(tag.GetString() ?? "")}";
                        }
                        else if (type == "app.bsky.richtext.facet#mention" && feature.TryGetProperty("did", out var did))
                        {
                            url = $"https://bsky.app/profile/{Uri.EscapeDataString(did.GetString() ?? "")}";
                        }
                    }

                    // Only ever emit web links, never javascript: or other schemes
                    if (url != null &&
                        (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) &&
                        bs.GetInt32() >= 0 && be.GetInt32() <= bytes.Length && bs.GetInt32() < be.GetInt32())
                    {
                        links.Add((bs.GetInt32(), be.GetInt32(), url));
                    }
                }
            }

            var sb = new StringBuilder("<p>");
            var pos = 0;
            foreach (var (start, end, url) in links.OrderBy(l => l.Start))
            {
                if (start < pos)
                {
                    continue; // overlapping facet
                }

                sb.Append(EncodeText(Encoding.UTF8.GetString(bytes, pos, start - pos)));
                sb.Append("<a href=\"").Append(WebUtility.HtmlEncode(url)).Append("\" target=\"_blank\" rel=\"noopener\">")
                  .Append(EncodeText(Encoding.UTF8.GetString(bytes, start, end - start)))
                  .Append("</a>");
                pos = end;
            }
            sb.Append(EncodeText(Encoding.UTF8.GetString(bytes, pos, bytes.Length - pos)));
            sb.Append("</p>");
            return sb.ToString();
        }

        private static string EncodeText(string s) =>
            WebUtility.HtmlEncode(s).Replace("\r\n", "\n").Replace("\n", "<br />");
    }
}
