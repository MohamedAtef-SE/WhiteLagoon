using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class VillaNumberServices : IVillaNumberServices
    {
        private IGenericRepository<VillaNumber> _villaNumberRepository;
        private IGenericRepository<Villa> _villaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _villaNumberRepository = _unitOfWork.GetGenericRepository<VillaNumber>();
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
        }

        public async Task<IEnumerable<VillaNumber>> GetAllAsync(Expression<Func<VillaNumber, bool>>? filter = null)
        {
            return await _villaNumberRepository.GetAll(filter,includeProperties: $"Villa").ToListAsync();
        }
        public async Task<VillaNumber?> GetAsync(int villaNumberId)
        {
            return await _villaNumberRepository.GetAsync(vn => vn.Villa_Number == villaNumberId);
        }

        public async Task AddAsync(VillaNumber villaNumber)
        {
            await _villaNumberRepository.AddAsync(villaNumber);
        }

        public bool Update(VillaNumber villaNumber)
        {
           return _villaNumberRepository.Update(villaNumber);
        }

        public bool Delete(VillaNumber villaNumber)
        {
            return _villaNumberRepository.Delete(villaNumber);
        }
    }
}
