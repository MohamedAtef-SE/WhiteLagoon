using Microsoft.AspNetCore.Identity;
using WhiteLagoon.Application._Common.Utility;

namespace WhiteLagoon.Web.Extentions
{
    public static class InitializationExtenstion
    {
        public async static Task InitializeAsync(this WebApplication app)
        {
            var scope =  app.Services.CreateAsyncScope();
            var service = scope.ServiceProvider;


            // Initialize Roles Table with Admin & Customer Roles.
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
            }
        }
    }
}
