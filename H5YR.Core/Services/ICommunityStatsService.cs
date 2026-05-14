using H5YR.Core.Models.ViewModels;

namespace H5YR.Core.Services
{
    public interface ICommunityStatsService
    {
        Task<CommunityStatsViewModel> GetStats();
    }
}
