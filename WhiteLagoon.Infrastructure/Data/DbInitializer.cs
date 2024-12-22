using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbInitializer(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task UpdateDataBaseAsync()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SeedDataAsync()
        {
            try
            {
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));

                    ApplicationUser user = new ApplicationUser()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Email = "admin@admin.com",
                        UserName = "admin@admin.com",
                        NormalizedUserName = "ADMIN@ADMIN.COM",
                        NormalizedEmail = "ADMIN@ADMIN.COM",
                        Name = "Mohamed Atef",
                        PhoneNumber = "01122334455"
                    };
                    await _userManager.CreateAsync(user, "Admin123*");
                    var admin = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.AddToRoleAsync(admin, SD.Role_Admin);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
