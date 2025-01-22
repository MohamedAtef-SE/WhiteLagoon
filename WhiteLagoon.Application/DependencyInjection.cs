using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interfaces;

namespace WhiteLagoon.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAmenityServices, AmenityServices>();

            services.AddScoped<IHomeServices, HomeServices>();
            services.AddScoped<IVillaNumberServices, VillaNumberServices>();

            services.AddScoped<IBookingServices, BookingServices>();
            services.AddScoped<Func<IBookingServices>>(provider =>
            {
                return () => provider.GetRequiredService<IBookingServices>();
            });

            services.AddScoped<IDashboardServices, DashboardServices>();
            services.AddScoped<Func<IDashboardServices>>(provider =>
            {
                return () => provider.GetRequiredService<IDashboardServices>();
            });

            services.AddScoped<IVillaServices, VillaServices>();
            services.AddScoped<Func<IVillaServices>>(provider =>
            {
                return () => provider.GetRequiredService<IVillaServices>();
            });

            services.AddScoped<IServiceManager, ServiceManager>();
            return services;
        }
    }
}
