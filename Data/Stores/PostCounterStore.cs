using h5yr.Data.Entities;
using h5yr.Data.Interfaces;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;

namespace h5yr.Data.Stores
{
    public class PostCounterStore : IPostCounterStore
    {
        private readonly ILogger<PostCounterStore> _logger;
        private IScopeProvider _scopeProvider;

        public PostCounterStore(ILogger<PostCounterStore> logger, IScopeProvider scopeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public IEnumerable<PostCounter> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var tweetCounter = scope.Database.Fetch<PostCounter>();
                scope.Complete();
                return tweetCounter;   
            }
        }

        public void Save(PostCounter poco)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Save(poco);
                scope.Complete();
            }
        }

        public void Update(PostCounter poco)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var existingCount = GetAll().LastOrDefault();

                if (existingCount != null)
                {
                    _logger.LogInformation("Updating latest value with quantity: " + poco.Quantity);

                    existingCount.Date = poco.Date;
                    existingCount.Quantity = poco.Quantity;
                    scope.Database.Update(existingCount);
                    scope.Complete();
                }
            }
        }

        public void Delete(PostCounter poco)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Delete(poco);
                scope.Complete();
            }
        }

        public int GetPostCount()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var total = GetAll().LastOrDefault()?.Quantity ?? 0;
                scope.Complete();
                return total;
            }
        }

    }
}
