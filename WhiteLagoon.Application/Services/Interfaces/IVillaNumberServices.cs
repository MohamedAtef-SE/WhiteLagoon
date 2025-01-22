using System.Linq.Expressions;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IVillaNumberServices
    {
        Task<IEnumerable<VillaNumber>> GetAllAsync(Expression<Func<VillaNumber,bool>>? filter = null);
        Task<VillaNumber?> GetAsync(int villaNumberId);
        Task AddAsync(VillaNumber villaNumber);
        bool Update(VillaNumber villaNumber);
        bool Delete(VillaNumber villaNumber);
    }
}
