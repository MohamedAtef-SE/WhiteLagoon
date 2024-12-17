using System.Linq.Expressions;

namespace WhiteLagoon.Application.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity,bool>>? filter = null,string? includeProperties = null);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null);
        Task<bool> AddAsync(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);

    }
}
