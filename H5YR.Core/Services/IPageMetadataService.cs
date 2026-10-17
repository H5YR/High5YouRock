namespace H5YR.Core.Services
{
    public record PageMetadata(string? Title, string? Description);

    public interface IPageMetadataService
    {
        /// <summary>
        /// Fetch the title and description (og:/twitter:/meta/title tag) of a public web page.
        /// Returns null if the page cannot be fetched or has no usable metadata.
        /// </summary>
        Task<PageMetadata?> GetMetadataAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}
