using H5YR.Core.Data.Constants;
using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Scoping;

namespace H5YR.Core.Data.Stores
{
    public class FeedItemLogStore : IFeedItemLogStore
    {
        private readonly ILogger<FeedItemLogStore> _logger;
        private readonly IScopeProvider _scopeProvider;

        public FeedItemLogStore(ILogger<FeedItemLogStore> logger, IScopeProvider scopeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public IEnumerable<FeedItemLog> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var items = scope.Database.Fetch<FeedItemLog>();
                scope.Complete();
                return items;
            }
        }

        public bool Exists(string externalId)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var count = scope.Database.ExecuteScalar<int>(
                    $"SELECT COUNT(1) FROM {FeedItemLogSchemaConstants.TableName} WHERE {FeedItemLogSchemaConstants.ExternalId} = @0",
                    externalId);
                scope.Complete();
                return count > 0;
            }
        }

        public void Save(FeedItemLog poco)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Insert(poco);
                scope.Complete();
            }
        }

        public void SaveRange(IEnumerable<FeedItemLog> items)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                foreach (var item in items)
                {
                    scope.Database.Insert(item);
                }
                scope.Complete();
            }
        }

        public void Delete(FeedItemLog poco)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Delete(poco);
                scope.Complete();
            }
        }
    }
}
