using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IAmenityServices
    {
        Task<IEnumerable<Amenity>> GetAmenitiesAsync();
        Task<Amenity?> GetAsync(int amenityId);
        Task<bool> AddAsync(Amenity amenity);
        bool Update(Amenity amenity);
        bool Delete(Amenity amenity);
    }
}
