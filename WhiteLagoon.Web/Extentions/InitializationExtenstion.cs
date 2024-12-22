using WhiteLagoon.Application.Interfaces;

namespace WhiteLagoon.Web.Extentions
{
    public static class InitializationExtenstion
    {
        public async static Task InitializeAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

            await dbInitializer.UpdateDataBaseAsync();
            await dbInitializer.SeedDataAsync();
        }
    }
}
