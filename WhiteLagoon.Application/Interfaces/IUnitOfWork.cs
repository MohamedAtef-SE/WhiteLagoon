namespace WhiteLagoon.Application.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;

        Task<bool> CompleteAsync();
    }

}
