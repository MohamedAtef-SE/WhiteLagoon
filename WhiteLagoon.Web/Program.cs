using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities.Identity;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repositories;
using WhiteLagoon.Infrastructure.UnitOfWork;
using WhiteLagoon.Web.Extentions;
using WhiteLagoon.Web.Mapping;

namespace WhiteLagoon.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o => 
            {
                o.Password.RequireUppercase = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = 8;
                o.Password.RequireNonAlphanumeric = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(configureCookie =>
            {
                //configureCookie.AccessDeniedPath = "/Account/AccessDenied"; // Default path by .Net no need to configure it
                //configureCookie.LoginPath = "/Account/Login"; // Default path no need to configure it
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddScoped<IBookingRepository,BookingRepository>();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            await app.InitializeAsync();

            app.Run();
        }
    }
}
