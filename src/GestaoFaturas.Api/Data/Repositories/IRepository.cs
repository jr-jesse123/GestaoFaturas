using System.Linq.Expressions;
using GestaoFaturas.Api.Data.Specifications;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    // Read operations
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        string includeProperties = "",
        CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        string includeProperties = "",
        CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Write operations
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);

    // Queryable for complex queries
    IQueryable<TEntity> Query(bool trackChanges = true);
    IQueryable<TEntity> QueryWithIncludes(string includeProperties, bool trackChanges = true);
    
    // Specification pattern support
    Task<TEntity?> GetEntityWithSpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
}