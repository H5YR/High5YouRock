using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Interfaces
{
    public interface IPostCounterStore
    {
        IEnumerable<PostCounter> GetAll();
        void Save(PostCounter poco);
        void Update(PostCounter poco);
        void Delete(PostCounter poco);
        int GetPostCount();
    }
}
