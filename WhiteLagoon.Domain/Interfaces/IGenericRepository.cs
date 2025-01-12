using System.Linq.Expressions;

namespace WhiteLagoon.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null, bool tracked = false);
        Task<bool> AddAsync(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);

    }
}
