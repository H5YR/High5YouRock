using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace H5YR.Core.Services
{
    /// <summary>
    /// Fetches page title/description for submitted URLs. Because the URL is user supplied,
    /// connections to loopback/private/link-local addresses are refused (including after redirects).
    /// </summary>
    public partial class PageMetadataService : IPageMetadataService
    {
        private const int MaxBytes = 256 * 1024;
        private const int MaxTitleLength = 200;
        private const int MaxDescriptionLength = 400;

        private static readonly HttpClient Client = CreateClient();

        private readonly ILogger<PageMetadataService> _logger;

        public PageMetadataService(ILogger<PageMetadataService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PageMetadata?> GetMetadataAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.UserAgent.ParseAdd("H5YR-LinkPreview/1.0 (+https://h5yr.com)");
                request.Headers.Accept.ParseAdd("text/html");

                using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var mediaType = response.Content.Headers.ContentType?.MediaType;
                if (mediaType != null && !mediaType.Contains("html", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var html = await ReadLimitedAsync(response, cancellationToken);
                return Parse(html);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Could not fetch page metadata for {Url}", uri);
                return null;
            }
        }

        internal static PageMetadata? Parse(string html)
        {
            var head = html;
            var headEnd = head.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            if (headEnd > 0)
            {
                head = head[..headEnd];
            }

            var title = GetMeta(head, "og:title") ?? GetMeta(head, "twitter:title");
            if (string.IsNullOrWhiteSpace(title))
            {
                var m = TitleRegex().Match(head);
                title = m.Success ? m.Groups[1].Value : null;
            }

            var description = GetMeta(head, "og:description")
                ?? GetMeta(head, "twitter:description")
                ?? GetMeta(head, "description");

            title = Clean(title, MaxTitleLength);
            description = Clean(description, MaxDescriptionLength);

            return title == null && description == null ? null : new PageMetadata(title, description);
        }

        private static string? GetMeta(string head, string key)
        {
            foreach (Match tag in MetaTagRegex().Matches(head))
            {
                var attrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (Match a in AttrRegex().Matches(tag.Value))
                {
                    attrs[a.Groups[1].Value] = a.Groups[2].Success ? a.Groups[2].Value : a.Groups[3].Value;
                }

                var name = attrs.GetValueOrDefault("property") ?? attrs.GetValueOrDefault("name");
                if (name != null && name.Equals(key, StringComparison.OrdinalIgnoreCase) &&
                    attrs.TryGetValue("content", out var content) && !string.IsNullOrWhiteSpace(content))
                {
                    return content;
                }
            }

            return null;
        }

        private static string? Clean(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var text = WebUtility.HtmlDecode(value);
            text = WhitespaceRegex().Replace(text, " ").Trim();
            if (text.Length == 0)
            {
                return null;
            }

            return text.Length > maxLength ? text[..(maxLength - 1)].TrimEnd() + "…" : text;
        }

        private static async Task<string> ReadLimitedAsync(HttpResponseMessage response, CancellationToken ct)
        {
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            var buffer = new byte[MaxBytes];
            var total = 0;
            int read;
            while (total < MaxBytes && (read = await stream.ReadAsync(buffer.AsMemory(total, MaxBytes - total), ct)) > 0)
            {
                total += read;
            }

            var encoding = Encoding.UTF8;
            var charset = response.Content.Headers.ContentType?.CharSet?.Trim('"');
            if (!string.IsNullOrEmpty(charset))
            {
                try { encoding = Encoding.GetEncoding(charset); } catch (ArgumentException) { }
            }

            return encoding.GetString(buffer, 0, total);
        }

        private static HttpClient CreateClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 3,
                ConnectTimeout = TimeSpan.FromSeconds(5),
                ConnectCallback = async (context, ct) =>
                {
                    var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host, ct);
                    var allowed = addresses.Where(IsPublicAddress).ToArray();
                    if (allowed.Length == 0)
                    {
                        throw new HttpRequestException("Target host resolves to a non-public address.");
                    }

                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
                    try
                    {
                        await socket.ConnectAsync(allowed, context.DnsEndPoint.Port, ct);
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch
                    {
                        socket.Dispose();
                        throw;
                    }
                }
            };

            return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(8) };
        }

        private static bool IsPublicAddress(IPAddress ip)
        {
            if (ip.IsIPv4MappedToIPv6)
            {
                ip = ip.MapToIPv4();
            }

            if (IPAddress.IsLoopback(ip) || ip.Equals(IPAddress.Any) || ip.Equals(IPAddress.IPv6Any))
            {
                return false;
            }

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return !(ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6Multicast || ip.IsIPv6UniqueLocal);
            }

            var b = ip.GetAddressBytes();
            return !(b[0] == 10
                || b[0] == 0
                || (b[0] == 172 && b[1] >= 16 && b[1] <= 31)
                || (b[0] == 192 && b[1] == 168)
                || (b[0] == 169 && b[1] == 254)
                || (b[0] == 100 && b[1] >= 64 && b[1] <= 127)
                || b[0] >= 224);
        }

        [GeneratedRegex("<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex TitleRegex();

        [GeneratedRegex("<meta\\s[^>]*>", RegexOptions.IgnoreCase)]
        private static partial Regex MetaTagRegex();

        [GeneratedRegex("([a-zA-Z:_-]+)\\s*=\\s*(?:\"([^\"]*)\"|'([^']*)')")]
        private static partial Regex AttrRegex();

        [GeneratedRegex("\\s+")]
        private static partial Regex WhitespaceRegex();
    }
}
