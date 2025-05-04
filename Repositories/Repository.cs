using DentalClinicAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ClinicContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ClinicContext context) // Mudamos para ClinicContext específico em vez de DbContext genérico
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll() => await _dbSet.ToListAsync();

        public async Task<T?> GetById(int id) => await _dbSet.FindAsync(id);

        public async Task Create(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}