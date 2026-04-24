using System.Linq.Expressions;

namespace TopGear.Application.Interfaces;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll(bool trackChanges = false);
    Task<List<T>> FindAllAsync(bool trackChanges = false);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
    T? GetById(int id);
    Task<T?> GetByIdAsync(int id);
    Task<bool> GetExistenceAsync(Expression<Func<T, bool>> expression);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
    void AddRange(IEnumerable<T> entities);
    void DeleteRange(IEnumerable<T> entities);
}
