using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public AmenityController(IServiceManager serviceManager, IMapper mapper)
        {
            _serviceManager = serviceManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var amenites = await _serviceManager.AmenityServices.GetAmenitiesAsync();
            if (amenites is null)
                return NotFound();
            var mappedAmenites = _mapper.Map<IEnumerable<AmenityViewModel>>(amenites);
            return View(mappedAmenites);
        }

        [HttpGet]
        public IActionResult Create()
        {
            AmenityViewModel amenityViewModel = new();

            return View(amenityViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AmenityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var mappedAmenity = _mapper.Map<Amenity>(model);
            var added = await _serviceManager.AmenityServices.AddAsync(mappedAmenity);

            if (!added)
                return BadRequest();

            var result = await _serviceManager.CompleteAsync();
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
            var amenity = await _serviceManager.AmenityServices.GetAsync(amenityId);
            if (amenity is null)
                return NotFound();

            var mappedAmenity = _mapper.Map<AmenityViewModel>(amenity);

            return View(mappedAmenity);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AmenityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var mappedAmenity = _mapper.Map<Amenity>(model);

            var updated = _serviceManager.AmenityServices.Update(mappedAmenity);
            if (!updated) return BadRequest();
            var result = await _serviceManager.CompleteAsync();

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
            var amenity = await _serviceManager.AmenityServices.GetAsync(amenityId);

            if (amenity is null) return NotFound();

            var mappedAmenity = _mapper.Map<AmenityViewModel>(amenity);

            return View(mappedAmenity);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AmenityViewModel model)
        {
            var amenity = await _serviceManager.AmenityServices.GetAsync(model.Id);
            if (amenity is null) return NotFound();
            var deleted = _serviceManager.AmenityServices.Delete(amenity);
            if (!deleted) return BadRequest();

            var result = await _serviceManager.CompleteAsync();
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
