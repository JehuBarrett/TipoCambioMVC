using System.Linq.Expressions;

namespace TipoCambioMVC.Data;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    void Update(T entity);
    Task AddAsync(T entity);
    void Remove(T entity);
}
