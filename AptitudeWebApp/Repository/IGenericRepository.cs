using AptitudeWebApp.Entities;

namespace AptitudeWebApp.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Create(T entity);
        Task Update(T entity);
        Task<T> Delete(int id);
    }   
}
