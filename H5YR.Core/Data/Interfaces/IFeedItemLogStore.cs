using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Interfaces
{
    public interface IFeedItemLogStore
    {
        IEnumerable<FeedItemLog> GetAll();
        bool Exists(string externalId);
        void Save(FeedItemLog poco);
        void SaveRange(IEnumerable<FeedItemLog> items);
        void Delete(FeedItemLog poco);
    }
}
