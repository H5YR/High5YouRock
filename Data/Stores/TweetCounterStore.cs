using h5yr.Data.Entities;
using h5yr.Data.Interfaces;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core.Scoping;

namespace h5yr.Data.Stores
{
    public class TweetCounterStore : ITweetCounterStore
    {
        private readonly IDatabase _db;
        private readonly ILogger<TweetCounterStore> _logger;
        private ICoreScopeProvider _scopeProvider;

        public TweetCounterStore(IDatabase db, ILogger<TweetCounterStore> logger,ICoreScopeProvider scopeProvider)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public IEnumerable<TweetCounter> GetAll()
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                var tweetCounter = _db.Fetch<TweetCounter>();
                scope.Complete();
                return tweetCounter;   
            }
        }

        public IEnumerable<TweetCounter> GetTweetCount(DateTime date)
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                var dateTo = date.ToString("MM/dd/yyyy");
                var tweetCounter = _db.Fetch<TweetCounter>("where date BETWEEN '" + dateTo + "' AND '" + dateTo + "'");
                scope.Complete();
                return tweetCounter;
            }
        }

        public int GetTweetsCountTotalByDate(DateTime dateFrom, DateTime dateTo)
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                var tweetDateFrom = dateFrom.ToString("MM/dd/yyyy");
                var tweetDateTo = dateTo.ToString("MM/dd/yyyy");
                var tweetCounter = _db.Fetch<TweetCounter>("where date BETWEEN '" + tweetDateFrom + "' AND '" + tweetDateTo + "'");

                if(tweetCounter?.Any() ?? false)
                {
                    var total = tweetCounter.Sum(item => item.Quantity);
                    scope.Complete();
                    return total;
                }

                scope.Complete();
                return 0;
            }
        }

        public void Save(TweetCounter poco)
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                _db.Save(poco);
                scope.Complete();
            }
        }

        public void Update(TweetCounter poco)
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                var tweetCount = GetTweetCount(date);

                if (tweetCount?.Any() ?? false)
                {
                    _logger.LogInformation("Existing tweet count found, updating existing value with quantity: " + poco.Quantity);
                    
                    var row = tweetCount.FirstOrDefault();
                    row.Quantity = poco.Quantity;
                    _db.Update(row);
                    scope.Complete();
                }
                else
                {
                    _logger.LogInformation("No tweet count found, adding new value with quantity: " + poco.Quantity + " for Date: " + poco.Date.ToLongDateString());
                    _db.Save(poco);
                    scope.Complete();
                }
            }
        }

        public void Delete(TweetCounter poco)
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                _db.Delete(poco);
            }
        }
    }
}
