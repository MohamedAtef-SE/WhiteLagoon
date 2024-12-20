using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private IGenericRepository<VillaNumber> _villaNumberRepository;
        private IGenericRepository<Villa> _villaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _villaNumberRepository =  _unitOfWork.GetGenericRepository<VillaNumber>();
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
        }
        public async Task<IActionResult> Index()
        {
            var villasNumbers = await _villaNumberRepository.GetAll(null,$"Villa").ToListAsync();

            if(villasNumbers is not null)
                return View(villasNumbers);

            return BadRequest("No villa numbers found");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var villaList = await _villaRepository.GetAll().ToListAsync();

            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                
                VillaList = villaList.Select(v => new SelectListItem()
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VillaNumberVM villaNumberVM)
        {
            var villaList = await _villaRepository.GetAll().ToListAsync();
            var villaNumberisExist = await _villaNumberRepository.GetAsync(VN => VN.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);

            villaNumberVM.VillaList = villaList.Select(V => new SelectListItem()
            {
                Text = V.Name,
                Value = V.Id.ToString()
            });
            if (villaNumberisExist is { })
            {
                TempData["error"] = "Villa Number is already exist";  
                return View(villaNumberVM);
            }
            if (ModelState.IsValid)
            {

                await _villaNumberRepository.AddAsync(villaNumberVM.VillaNumber);
               var result = await _unitOfWork.CompleteAsync();

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
            var villaNumber = await _villaNumberRepository.GetAsync(V => V.Villa_Number == villaNumberId);

            if (villaNumber is null)
                return RedirectToAction("Error","Home");
            var villaList = await _villaRepository.GetAll().ToListAsync();
            VillaNumberVM vm = new VillaNumberVM()
            {
                VillaNumber = villaNumber,
                VillaList = villaList.Select(v => new SelectListItem()
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                })
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(VillaNumberVM updatedVillaNumberVM)
        {
            ModelState.Remove("Villa");
            if (!ModelState.IsValid)
                return View(updatedVillaNumberVM);

            
           var updated = _villaNumberRepository.Update(updatedVillaNumberVM.VillaNumber);

            var result = await _unitOfWork.CompleteAsync();

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
            var villaNumber = await _villaNumberRepository.GetAsync(V => V.Villa_Number == villaNumberId);
            if(villaNumber is null)
                return RedirectToAction("Error","Home");

            var villaList = await _villaRepository.GetAll().ToListAsync();
            VillaNumberVM vm = new VillaNumberVM()
            {
                VillaNumber = villaNumber,
                VillaList = villaList.Select(v => new SelectListItem()
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                })
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VillaNumberVM villaNumberVM)
        {
            var villaNumber = await _villaNumberRepository.GetAsync(V => V.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);

           if(villaNumber is null)
                return RedirectToAction("Error","Home");

            _villaNumberRepository.Delete(villaNumber);

            var result = await _unitOfWork.CompleteAsync();

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
