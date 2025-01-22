using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class HomeController(IServiceManager _serviceManager) : Controller
    {
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var villas = await _serviceManager.VillaServices.GetAllAsync();
            if(villas is null)
                return NotFound();

            var homeViewModel = new HomeViewModel()
            {
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Villas = villas
            };
            return View(homeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetVillasByDate(DateOnly checkInDate, int nights)
        {
            Thread.Sleep(500);
            var villas = await _serviceManager.VillaServices.GetVillasByDate(checkInDate, nights);

           if(villas is null) return BadRequest();

            HomeViewModel model = new HomeViewModel()
            {
                CheckInDate = checkInDate,
                Nights = nights,
                Villas = villas
            };
            return PartialView("Partial/VillaListPartialView", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
