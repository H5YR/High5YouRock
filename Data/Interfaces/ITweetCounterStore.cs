using h5yr.Data.Entities;

namespace h5yr.Data.Interfaces
{
    public interface ITweetCounterStore
    {
        IEnumerable<TweetCounter> GetAll();
        IEnumerable<TweetCounter> GetTweetCount(DateTime date);
        int GetTweetsCountTotalByDate(DateTime dateFrom, DateTime dateTo);
        void Save(TweetCounter poco);
        void Update(TweetCounter poco);
        void Delete(TweetCounter poco);
    }
}
