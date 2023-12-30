using AptitudeWebApp.DAL;
using AptitudeWebApp.Entities;
using AptitudeWebApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace AptitudeWebApp.Service
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AptitudeContext _db;
        private DbSet<T> _dbSet;
        public GenericRepository(AptitudeContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public async Task Create(T entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<T> Delete(int id)
        {
            var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                _dbSet.Remove(product);
                await _db.SaveChangesAsync();
                return product;
            }
            else
            {
                return null!;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
