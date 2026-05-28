using H5YR.Core.Models;

namespace H5YR.Core.Services
{
    public interface IUnifiedFeedService
    {
        Task<IEnumerable<UnifiedFeedItem>> GetUnifiedFeedAsync(int count = 12);
        int GetTotalCount();
    }
}