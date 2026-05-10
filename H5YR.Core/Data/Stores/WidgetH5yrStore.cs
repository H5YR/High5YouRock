using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;

namespace H5YR.Core.Data.Stores
{
    public class WidgetH5yrStore : IWidgetH5yrStore
    {
        private readonly ILogger<WidgetH5yrStore> _logger;
        private readonly IScopeProvider _scopeProvider;

        public WidgetH5yrStore(ILogger<WidgetH5yrStore> logger, IScopeProvider scopeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public IEnumerable<WidgetH5yr> GetAll()
        {
            using var scope = _scopeProvider.CreateScope();
            var items = scope.Database.Fetch<WidgetH5yr>("SELECT * FROM WidgetH5yr ORDER BY CreatedAt DESC");
            scope.Complete();
            return items;
        }

        public IEnumerable<WidgetH5yr> GetRecent(int count)
        {
            using var scope = _scopeProvider.CreateScope();
            // Use NPoco's Page() so it generates the correct LIMIT/TOP syntax per database dialect
            var page = scope.Database.Page<WidgetH5yr>(1, count,
                "SELECT * FROM WidgetH5yr ORDER BY CreatedAt DESC");
            scope.Complete();
            return page.Items;
        }

        public IEnumerable<WidgetH5yr> GetRecentBefore(DateTime before, int count)
        {
            using var scope = _scopeProvider.CreateScope();
            var page = scope.Database.Page<WidgetH5yr>(1, count,
                "SELECT * FROM WidgetH5yr WHERE CreatedAt < @0 ORDER BY CreatedAt DESC", before);
            scope.Complete();
            return page.Items;
        }

        public int GetSubmissionCountToday(string externalUserId, string authProvider, string targetSiteHost)
        {
            using var scope = _scopeProvider.CreateScope();
            var todayStart = DateTime.UtcNow.Date;
            var count = scope.Database.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM WidgetH5yr WHERE ExternalUserId = @0 AND AuthProvider = @1 AND TargetSiteHost = @2 AND CreatedAt >= @3",
                externalUserId, authProvider, targetSiteHost, todayStart);
            scope.Complete();
            return count;
        }

        public void Save(WidgetH5yr poco)
        {
            using var scope = _scopeProvider.CreateScope();
            scope.Database.Save(poco);
            scope.Complete();
        }

        public void Delete(WidgetH5yr poco)
        {
            using var scope = _scopeProvider.CreateScope();
            scope.Database.Delete(poco);
            scope.Complete();
        }

        public int GetTotalCount()
        {
            using var scope = _scopeProvider.CreateScope();
            var count = scope.Database.ExecuteScalar<int>("SELECT COUNT(*) FROM WidgetH5yr");
            scope.Complete();
            return count;
        }
    }
}
