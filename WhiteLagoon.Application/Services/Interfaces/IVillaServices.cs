using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IVillaServices
    {
        Task<IEnumerable<Villa>> GetAllAsync();
        Task<Villa?> GetAsync(int villaId);
        Task<IEnumerable<Villa>?> GetVillasByDate(DateOnly checkInDate, int nights);
        Task<bool> AddAsync(Villa villa);
        bool Update(Villa villa);
        bool Delete(Villa villa);
    }
}
