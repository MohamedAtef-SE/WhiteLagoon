using System.Collections.Concurrent;
using WhiteLagoon.Domain.Interfaces;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repositories;

namespace WhiteLagoon.Infrastructure.UnitOfWork
{
    public class UnitOfWork(ApplicationDbContext _dbContext) : IUnitOfWork
    {
        private ConcurrentDictionary<string,object> _concurrentDictionary = new ConcurrentDictionary<string,object>();
        public IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
        {
            return (IGenericRepository<TEntity>)  _concurrentDictionary.GetOrAdd(typeof(TEntity).Name, new GenericRepository<TEntity>(_dbContext));
        }

        public async Task<bool> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async ValueTask DisposeAsync()
        {
           await _dbContext.DisposeAsync();
        }
    }
}
