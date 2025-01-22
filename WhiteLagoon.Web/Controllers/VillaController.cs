using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.Helpers;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController(IServiceManager _serviceManager, IMapper _mapper) : Controller
    {
        
        public async Task<IActionResult> Index()
        {
            var villas = await _serviceManager.VillaServices.GetAllAsync();
            
            var mappedVillas = _mapper.Map<IEnumerable<VillaViewModel>>(villas);

            if (villas is null)
                return BadRequest("No villa found");

            return View(mappedVillas);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VillaViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            if (model.Image is not null)
            {
                model.ImageURL = ImageSettings<Villa>.GetImageURL(model.Image);
            }

            var mappedVilla = _mapper.Map<Villa>(model);

            await _serviceManager.VillaServices.AddAsync(mappedVilla);
            var result = await _serviceManager.CompleteAsync();

            if (!result)
            {
                TempData["error"] = "Creating failed";
            }
            else
            {
                TempData["success"] = "Created successfully";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Update(int villaId)
        {
            var villa = await _serviceManager.VillaServices.GetAsync(villaId);

            if (villa is null)
                return RedirectToAction("Error", "Home");

            var mappedVilla = _mapper.Map<VillaViewModel>(villa);

            return View(mappedVilla);
        }

        [HttpPost]
        public async Task<IActionResult> Update(VillaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Image is not null)
            {
                if (model.ImageURL is not null)
                {
                    ImageSettings<Villa>.DeleteFile(model.ImageURL);
                }
                model.ImageURL = ImageSettings<Villa>.GetImageURL(model.Image);
            }
           
            var mappedVilla = _mapper.Map<Villa>(model);

            _serviceManager.VillaServices.Update(mappedVilla);

            var result = await _serviceManager.CompleteAsync();

            if (!result)
            {
                TempData["error"] = "Updating failed";
            }
            else
            {
                TempData["success"] = "Updated successfully";
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int villaId)
        {
            var villa = await _serviceManager.VillaServices.GetAsync(villaId);
            if (villa is null)
                return RedirectToAction("Error", "Home");

            var mappedVilla = _mapper.Map<VillaViewModel>(villa);

            return View(mappedVilla);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VillaViewModel model)
        {
            var villa = await _serviceManager.VillaServices.GetAsync(model.Id);

            if (villa is null)
                return RedirectToAction("Error", "Home");

            _serviceManager.VillaServices.Delete(villa);

            var result = await _serviceManager.CompleteAsync();

            if (!result)
            {
                TempData["error"] = "Deleting failed";
            }
            else
            {
                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction("Index");
        }

    }
}
