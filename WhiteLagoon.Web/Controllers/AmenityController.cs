using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IGenericRepository<Amenity> _amenityRepository;
        private readonly IGenericRepository<Villa> _villaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AmenityController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _amenityRepository = _unitOfWork.GetGenericRepository<Amenity>();
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
            
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var amenites = await _amenityRepository.GetAll(null, "Villa").ToListAsync();
            if(amenites is null)
                return NotFound();
            var mappedAmenites =_mapper.Map<IEnumerable<AmenityViewModel>>(amenites);
            return View(mappedAmenites);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var villas = await _villaRepository.GetAll().ToListAsync();
            var amenityViewModel = new AmenityViewModel()
            {
                Name = "",
                VillaList = villas.Select(villa => new SelectListItem()
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                })
            };
            return View(amenityViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AmenityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var villas = await _villaRepository.GetAll().ToListAsync();
                model.VillaList = villas.Select(villa => new SelectListItem()
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                });
                return View(model);
            }
            
            var mappedAmenity = _mapper.Map<Amenity>(model);
            var added = await _amenityRepository.AddAsync(mappedAmenity);

            if(!added)
                return BadRequest();

            var result = await _unitOfWork.CompleteAsync();
            if (result)
            {
                TempData["success"] = "Created successfully";
            }
            else
            {
                TempData["error"] = "Creating failed";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int amenityId)
        {
            var amenity = await _amenityRepository.GetAsync(amenity => amenity.Id == amenityId);
            if (amenity is null)
                return NotFound();

            var mappedAmenity = _mapper.Map<AmenityViewModel>(amenity);
            var villas = await _villaRepository.GetAll().ToListAsync();
            mappedAmenity.VillaList = villas.Select(villa => new SelectListItem()
            {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });
            return View(mappedAmenity);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AmenityViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            var mappedAmenity = _mapper.Map<Amenity>(model);
            var updated = _amenityRepository.Update(mappedAmenity);
            if (!updated) return BadRequest();
            var result = await _unitOfWork.CompleteAsync();

            if (result)
            {
                TempData["success"] = "Updated successfully";
            }
            else
            {
                TempData["error"] = "Updating failed";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int amenityId)
        {
            var amenity = await _amenityRepository.GetAsync(amenity => amenity.Id == amenityId);   

            if(amenity is null) return NotFound();

            var mappedAmenity = _mapper.Map<AmenityViewModel>(amenity);
            var villas = await _villaRepository.GetAll().ToListAsync();
            mappedAmenity.VillaList = villas.Select(villa => new SelectListItem()
            {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });
            return View(mappedAmenity);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AmenityViewModel model)
        {
            var amenity =await _amenityRepository.GetAsync(amenity => amenity.Id == model.Id);
            if (amenity is null) return NotFound();
            var deleted = _amenityRepository.Delete(amenity);
            if (!deleted) return BadRequest();

            var result = await _unitOfWork.CompleteAsync();
            if (result)
            {
                TempData["success"] = "Deleted successfully";
            }
            else
            {
                TempData["error"] = "Deleting failed";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
