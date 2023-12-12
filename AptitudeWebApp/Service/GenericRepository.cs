using AptitudeWebApp.DAL;
using AptitudeWebApp.Entities;
using AptitudeWebApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace AptitudeWebApp.Service
{
    public class GenericRepository<I> : IGenericRepository<I> where I : BaseEntity
    {
        private readonly AptitudeContext _db;
        private DbSet<I> _dbSet;
        public GenericRepository(AptitudeContext db)
        {
            _db = db;
            _dbSet = db.Set<I>();
        }

        public async Task Create(I entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<I> Delete(int id)
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

        public async Task<IEnumerable<I>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<I> GetById(int id)
        {
            return await _dbSet.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task Update(I entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
