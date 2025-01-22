using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Interfaces;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class VillaServices : IVillaServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<Villa> _villaRepository;
        private readonly IServiceManager _serviceManager;
        public VillaServices(IServiceManager serviceManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _serviceManager = serviceManager;
            _configuration = configuration;
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
        }

        public async Task<IEnumerable<Villa>> GetAllAsync()
        {
            var villas = await _villaRepository.GetAll().ToListAsync();
            return villas;
        }
        public async Task<Villa?> GetAsync(int villaId)
        {
            return await _villaRepository.GetAsync(V => V.Id == villaId);
        }

        public async Task<IEnumerable<Villa>?> GetVillasByDate(DateOnly checkInDate, int nights)
        {
            var villas = await GetAllAsync();
            if (villas is null)
                return null;
            var villasNumber = await _serviceManager.VillaNumberServices.GetAllAsync();
            var bookedVillas = await _serviceManager.BookingServices.GetAllAsync(booking => booking.Status == SD.StatusApproved
                                                                          || booking.Status == SD.StatusCheckedIn);

            foreach (var villa in villas)
            {
                int roomsAvailable = SD.VillaRoomsAvailable_Count(villa.Id, villasNumber.ToList(), checkInDate, nights, bookedVillas.ToList());

                villa.IsAvailable = roomsAvailable > 0;
            }
            return villas;
        }

        public async Task<bool> AddAsync(Villa villa)
        {

            return await _villaRepository.AddAsync(villa);
        }

        public bool Update(Villa villa)
        {
            return _villaRepository.Update(villa);
        }

        public bool Delete(Villa villa)
        {
            return _villaRepository.Delete(villa);
        }
    }
}
