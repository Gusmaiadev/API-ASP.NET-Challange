namespace DentalClinicAPI.Repositories;
using DentalClinicAPI.Data;

using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ClinicContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ClinicContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll() => await _dbSet.ToListAsync();
    public async Task<T> GetById(int id) => await _dbSet.FindAsync(id);
    public async Task Create(T entity) => await _dbSet.AddAsync(entity);
    public async Task Update(T entity) => _dbSet.Update(entity);
    public async Task Delete(int id) => _dbSet.Remove(await GetById(id));
}