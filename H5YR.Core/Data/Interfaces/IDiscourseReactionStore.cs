using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Interfaces
{
    public interface IDiscourseReactionStore
    {
        IEnumerable<DiscourseReaction> GetAll();
        IEnumerable<DiscourseReaction> GetRecent(int count);
        DiscourseReaction? GetByPostId(int postId);
        void Save(DiscourseReaction reaction);
        void Delete(DiscourseReaction reaction);
        int GetTotalCount();
        bool ReactionExists(int postId);
    }
}