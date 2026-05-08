using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace H5YR.Core.Services
{
    public class HomeSettingsService
    {
        private readonly IDocumentNavigationQueryService _documentNavigationQueryService;
        private readonly IDocumentCacheService _documentCacheService;
        public HomeSettingsService(IDocumentNavigationQueryService documentNavigationQueryService,
            IDocumentCacheService documentCacheService)
        {
            _documentNavigationQueryService = documentNavigationQueryService;
            _documentCacheService = documentCacheService;
        }

        public async Task<Home?> SettingsAsync()
        {
            if (_documentNavigationQueryService.TryGetRootKeysOfType(Home.ModelTypeAlias, out IEnumerable<Guid> rootKeys))
            {
                return (await _documentCacheService.GetByKeyAsync(rootKeys.FirstOrDefault())) as Home;
            }
            return null;
        }
    }
}
