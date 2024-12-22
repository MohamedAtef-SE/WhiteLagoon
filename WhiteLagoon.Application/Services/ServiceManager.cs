using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interfaces;

namespace WhiteLagoon.Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAmenityServices> _amenityServices;
        private readonly Lazy<IBookingServices> _bookingServices;
        private readonly Lazy<IDashboardServices> _dashboardServices;
        private readonly Lazy<IHomeServices> _homeServices;
        private readonly Lazy<IVillaNumberServices> _villaNumberServices;
        private readonly Lazy<IVillaServices> _villaServices;
        private readonly IUnitOfWork _unitOfWork;


        public ServiceManager(Func<IVillaServices> villaServices, IUnitOfWork unitOfWork, Func<IBookingServices> bookingService, Func<IDashboardServices> dashboardService)
        {
            _unitOfWork = unitOfWork;
            _amenityServices = new Lazy<IAmenityServices>(() => new AmenityServices(_unitOfWork));
            _bookingServices = new Lazy<IBookingServices>(bookingService);
            _dashboardServices = new Lazy<IDashboardServices>(dashboardService);
            _homeServices = new Lazy<IHomeServices>(() => new HomeServices());
            _villaNumberServices = new Lazy<IVillaNumberServices>(() => new VillaNumberServices(unitOfWork));
            _villaServices = new Lazy<IVillaServices>(villaServices);
        }
        public IAmenityServices AmenityServices => _amenityServices.Value;

        public IBookingServices BookingServices => _bookingServices.Value;

        public IDashboardServices DashboardServices => _dashboardServices.Value;

        public IHomeServices HomeServices => _homeServices.Value;

        public IVillaNumberServices VillaNumberServices => _villaNumberServices.Value;

        public IVillaServices VillaServices => _villaServices.Value;

        public async Task<bool> CompleteAsync()
        {
            return await _unitOfWork.CompleteAsync();
        }
    }
}
