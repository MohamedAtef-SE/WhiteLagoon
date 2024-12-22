using WhiteLagoon.Application.Services.Interfaces;

namespace WhiteLagoon.Application.Services
{
    public interface IServiceManager
    {
        IAmenityServices AmenityServices { get; }
        IBookingServices BookingServices { get; }
        IDashboardServices DashboardServices { get; }
        IHomeServices HomeServices { get; }
        IVillaNumberServices VillaNumberServices { get; }
        IVillaServices VillaServices { get; }
        Task<bool> CompleteAsync();
    }
}
