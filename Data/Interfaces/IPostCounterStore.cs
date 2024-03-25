using h5yr.Data.Entities;

namespace h5yr.Data.Interfaces
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
