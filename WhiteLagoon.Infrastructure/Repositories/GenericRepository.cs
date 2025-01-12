using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WhiteLagoon.Domain.Interfaces;
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
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<TEntity> query;
            if (tracked)
            {
                query = _dbContext.Set<TEntity>();
            }
            else
            {
                query = _dbContext.Set<TEntity>().AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<TEntity> query;
            if (tracked)
            {
                query = _dbContext.Set<TEntity>().Where(filter);
            }
            else
            {
                query = _dbContext.Set<TEntity>().Where(filter).AsNoTracking();
            }

            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
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
