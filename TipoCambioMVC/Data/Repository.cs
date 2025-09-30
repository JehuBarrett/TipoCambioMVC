using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace TipoCambioMVC.Data;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _ctx;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext ctx)
    {
        _ctx = ctx;
        _dbSet = _ctx.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) =>  _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);
}
