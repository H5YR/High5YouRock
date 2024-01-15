using h5yr.ViewComponents;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace h5yr.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceCustomEmojis(this string text, IReadOnlyList<MastodonCustomEmoji> customEmojis)
        {
            foreach (var emoji in customEmojis)
            {
                text = text.Replace($":{emoji.Shortcode}:", $"<img src='{emoji.Url}' alt='{emoji.Shortcode}' title='{emoji.Shortcode}' class='inline_emoji' />");
            }

            return text;
        }
    }
}
