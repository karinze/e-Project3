using AptitudeWebApp.Entities;

namespace AptitudeWebApp.Repository
{
    public interface IGenericRepository<I> where I : BaseEntity
    {
        Task<IEnumerable<I>> GetAll();
        Task<I> GetById(int id);
        Task Create(I entity);
        Task Update(I entity);
        Task<I> Delete(int id);
    }   
}
