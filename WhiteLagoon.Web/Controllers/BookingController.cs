using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {


        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IServiceManager _serviceManager; 
        public BookingController(IServiceManager serviceManager, UserManager<ApplicationUser> userManager)
        {
            _serviceManager = serviceManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var user = await _userManager.GetUserAsync(User);

            var villa = await _serviceManager.VillaServices.GetAsync(villaId);
            if (villa is null)
                return BadRequest();

            Booking booking = new Booking()
            {
                UserId = user!.Id,
                Name = user.Name,
                Email = user!.Email!,
                Phone = user.PhoneNumber,
                CheckInDate = checkInDate,
                VillaId = villaId,
                Villa = villa,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights)
            };
            booking.TotalCost = booking.Nights * villa.Price;
            return View(booking);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FinalizeBooking(Booking booking)
        {
            var result = await _serviceManager.BookingServices.FinalizeBooking(booking, Request, Response);

            if (result.StatusCode == 404)
            {
                TempData["error"] = "Room has been sold out!";
                // no room available
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights,
                });
            }
            else if (result.StatusCode == 303)
            {
                TempData["success"] = "Booked successfully";
                return result;
            }
            else if (result.StatusCode == 400)
            {
                TempData["error"] = "Booking failed";
                return View(booking);
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            var result = await _serviceManager.BookingServices.BookingConfirmation(bookingId);
            if (result is null)
                return BadRequest();

            TempData["success"] = "Payment successful";
            return View(bookingId);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            Booking? booking = await _serviceManager.BookingServices.BookingDetails(bookingId);
            if (booking is null)
                return NotFound();

            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> CheckIn(Booking booking)
        {
            var result = await _serviceManager.BookingServices.CheckInAsync(booking);
            if (result is null)
            {
                TempData["error"] = "Updating Booking failed";
            }
            else
            {
                TempData["success"] = "Booking Updated successfully";
            }
            return RedirectToAction(nameof(BookingDetails), new { bookingId = result });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> CheckOut(Booking booking)
        {
            var result = await _serviceManager.BookingServices.CheckOutAsync(booking);
            if (result is null)
            {
                TempData["error"] = "Checkout Booking failed";
            }
            else
            {
                TempData["success"] = "Booking Completed successfully";
            }
            return RedirectToAction(nameof(BookingDetails), new { bookingId = result });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> CancelBooking(Booking booking)
        {
            var result = await _serviceManager.BookingServices.CancelAsync(booking);
            if (result is null)
            {
                TempData["error"] = "Booking Cancellation failed";
            }
            else
            {
                TempData["success"] = "Booking Cancelled successfully";
            }
            return RedirectToAction(nameof(BookingDetails), new { bookingId = result });
        }


        #region API Calls

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<Booking> bookings = await _serviceManager.BookingServices.GetAllAsync(status, User);
            return Json(new { data = bookings });
        }

        #endregion
    }
}
