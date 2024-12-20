using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.Helpers;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGenericRepository<Villa> _villaRepository;
        public VillaController(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
        }
        public async Task<IActionResult> Index()
        {
            var villas = await _villaRepository.GetAll().ToListAsync();
            
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

            await _villaRepository.AddAsync(mappedVilla);
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


        [HttpGet]
        public async Task<IActionResult> Update(int villaId)
        {
            var villa = await _villaRepository.GetAsync(V => V.Id == villaId);

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

            _villaRepository.Update(mappedVilla);

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
        public async Task<IActionResult> Delete(int villaId)
        {
            var villa = await _villaRepository.GetAsync(V => V.Id == villaId);
            if (villa is null)
                return RedirectToAction("Error", "Home");

            var mappedVilla = _mapper.Map<VillaViewModel>(villa);

            return View(mappedVilla);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VillaViewModel model)
        {
            var villa = await _villaRepository.GetAsync(V => V.Id == model.Id);

            if (villa is null)
                return RedirectToAction("Error", "Home");

            _villaRepository.Delete(villa);

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
