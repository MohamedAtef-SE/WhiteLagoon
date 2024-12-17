using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(",",StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().Where(filter);

            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                   query =  query.Include(property);
                }
                
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<bool> AddAsync(TEntity entity)
        {
            var result = await _dbContext.Set<TEntity>().AddAsync(entity);
            return result.State == EntityState.Added;
        }

        public bool Update(TEntity entity)
        {
            var result = _dbContext.Set<TEntity>().Update(entity);
            return result.State == EntityState.Modified;
        }

        public bool Delete(TEntity entity)
        {
           var result = _dbContext.Set<TEntity>().Remove(entity);
            return result.State == EntityState.Deleted;
        }

    }
}
