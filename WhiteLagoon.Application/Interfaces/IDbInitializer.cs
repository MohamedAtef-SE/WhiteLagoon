namespace WhiteLagoon.Application.Interfaces
{
    public interface IDbInitializer
    {
        Task UpdateDataBaseAsync();
        Task SeedDataAsync();
    }
}
