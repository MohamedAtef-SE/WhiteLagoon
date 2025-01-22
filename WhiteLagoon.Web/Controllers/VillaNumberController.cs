using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController(IServiceManager _serviceManager) : Controller
    {

        public async Task<IActionResult> Index()
        {

            var villaNumbers = await _serviceManager.VillaNumberServices.GetAllAsync();
            if (villaNumbers is not null)
                return View(villaNumbers);

            return BadRequest("No villa numbers found");
        }

        [HttpGet]
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new();
            return View(villaNumberVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VillaNumberVM villaNumberVM)
        {
            var villaNumberisExist = await _serviceManager.VillaNumberServices.GetAsync(villaNumberVM.VillaNumber.Villa_Number);

            if (villaNumberisExist is { })
            {
                TempData["error"] = "Villa Number is already exist";
                return View(villaNumberVM);
            }

            if (ModelState.IsValid)
            {
                await _serviceManager.VillaNumberServices.AddAsync(villaNumberVM.VillaNumber);
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
            else
            {
                ModelState.AddModelError("", ModelState.Values.Select(S => S.Errors.Select(E => E.ErrorMessage).FirstOrDefault()).FirstOrDefault());
            }

            return View(villaNumberVM);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int villaNumberId)
        {
            var villaNumber = await _serviceManager.VillaNumberServices.GetAsync(villaNumberId);

            if (villaNumber is null)
                return RedirectToAction("Error", "Home");

            VillaNumberVM vm = new VillaNumberVM()
            {
                VillaNumber = villaNumber
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(VillaNumberVM updatedVillaNumberVM)
        {
            ModelState.Remove("Villa");
            if (!ModelState.IsValid)
                return View(updatedVillaNumberVM);


            var updated = _serviceManager.VillaNumberServices.Update(updatedVillaNumberVM.VillaNumber);

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
        public async Task<IActionResult> Delete(int villaNumberId)
        {
            var villaNumber = await _serviceManager.VillaNumberServices.GetAsync(villaNumberId);
            if (villaNumber is null)
                return RedirectToAction("Error", "Home");

            VillaNumberVM vm = new VillaNumberVM()
            {
                VillaNumber = villaNumber
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VillaNumberVM villaNumberVM)
        {
            var villaNumber = await _serviceManager.VillaNumberServices.GetAsync(villaNumberVM.VillaNumber.Villa_Number);

            if (villaNumber is null)
                return RedirectToAction("Error", "Home");

            _serviceManager.VillaNumberServices.Delete(villaNumber);

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
