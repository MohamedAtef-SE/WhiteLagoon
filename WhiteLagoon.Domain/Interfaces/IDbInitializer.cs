namespace WhiteLagoon.Domain.Interfaces
{
    public interface IDbInitializer
    {
        Task UpdateDataBaseAsync();
        Task SeedDataAsync();
    }
}
