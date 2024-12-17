using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.Models;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Villa> _villaRepository;
        private readonly IGenericRepository<VillaNumber> _villaNumberRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IMapper _mapper;
        public HomeController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
            _villaNumberRepository = _unitOfWork.GetGenericRepository<VillaNumber>();
            _bookingRepository = _unitOfWork.GetGenericRepository<Booking>();
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var villas = await _villaRepository.GetAllAsync(includeProperties:"Amenities");
            if(villas is null)
                return NotFound();

            //var mappedVillas = _mapper.Map<IEnumerable<VillaViewModel>>(villas);

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
            var villas = await _villaRepository.GetAllAsync(includeProperties: "Amenities");
            if (villas is null)
                return NotFound();
            var villasNumber = await _villaNumberRepository.GetAllAsync();
            var bookedVillas = await _bookingRepository.GetAllAsync(booking => booking.Status == SD.StatusApproved 
                                                                            || booking.Status == SD.StatusCheckedIn);

            foreach (var villa in villas)
            {
                int roomsAvailable = SD.VillaRoomsAvailable_Count(villa.Id,villasNumber.ToList(),checkInDate,nights,bookedVillas.ToList());

                villa.IsAvailable = roomsAvailable > 0;
            }
           // var mappedVillas = _mapper.Map<IEnumerable<VillaViewModel>>(villas);
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
