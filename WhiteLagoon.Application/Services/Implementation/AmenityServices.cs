using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class AmenityServices : IAmenityServices
    {
        private readonly IGenericRepository<Amenity> _amenityRepository;
        private readonly IGenericRepository<Villa> _villaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AmenityServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _amenityRepository = _unitOfWork.GetGenericRepository<Amenity>();
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();

        }
        public async Task<IEnumerable<Amenity>> GetAmenitiesAsync()
        {
            var amenites = await _amenityRepository.GetAll(includeProperties: "Villa").ToListAsync();
            return amenites;
        }

        public async Task<Amenity?> GetAsync(int amenityId)
        {
            return await _amenityRepository.GetAsync(amenity => amenity.Id == amenityId);
        }

        public async Task<bool> AddAsync(Amenity amenity)
        {
            return await _amenityRepository.AddAsync(amenity);
        }

        public bool Update(Amenity amenity)
        {
            return _amenityRepository.Update(amenity);
        }

        public bool Delete(Amenity amenity)
        {
            return _amenityRepository.Delete(amenity);
        }
    }
}
