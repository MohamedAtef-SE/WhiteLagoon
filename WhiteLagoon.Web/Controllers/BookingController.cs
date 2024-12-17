using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;
using WhiteLagoon.Web.DTOs;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IGenericRepository<Villa> _villaRepository;
        private readonly IGenericRepository<VillaNumber> _villaNumberRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IBookingRepository bookingRepository)
        {
            _unitOfWork = unitOfWork;
            _villaRepository = _unitOfWork.GetGenericRepository<Villa>();
            _villaNumberRepository = _unitOfWork.GetGenericRepository<VillaNumber>();
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var user = await _userManager.GetUserAsync(User);

            //var mappedUser = _mapper.Map<UserDTO>(user);

            var villa = await _villaRepository.GetAsync(v => v.Id == villaId, includeProperties: "Amenities");

           // var mappedVilla = _mapper.Map<VillaViewModel>(villa);
            Booking booking = new Booking()
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                CheckInDate = checkInDate,
                VillaId = villaId,
                Villa = villa,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights)
            };
            booking.TotalCost = booking.Nights * villa!.Price;
            return View(booking);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FinalizeBooking(Booking booking)
        {
            var villa = await _villaRepository.GetAsync(v => v.Id == booking.VillaId, includeProperties: "Amenities");
            booking.TotalCost = villa!.Price * booking.Nights;

            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.UtcNow;
           
            var villasNumber = await _villaNumberRepository.GetAllAsync();
            var bookedVillas = await _bookingRepository.GetAllAsync(booking => booking.Status == SD.StatusApproved
                                                                            || booking.Status == SD.StatusCheckedIn);



            int roomsAvailable = SD.VillaRoomsAvailable_Count((int)villa.Id, villasNumber.ToList(),
                                                              booking.CheckInDate, booking.Nights,
                                                              bookedVillas.ToList());

            if (roomsAvailable == 0)
            {
                TempData["error"] = "Room has been sold out!";
                // no room available
                return RedirectToAction(nameof(FinalizeBooking),new 
                {
                    villaId =booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights,
                });
            }



            var result = await _bookingRepository.AddAsync(booking);
            if (result)
            {
                var completed = await _unitOfWork.CompleteAsync();
                if (completed)
                {
                    #region Stripe

                    var domain = $"{Request.Scheme}://{Request.Host.Value}";

                    var options = new SessionCreateOptions()
                    {
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = $"{domain}/booking/BookingConfirmation?bookingId={booking.Id}",
                        CancelUrl = $"{domain}/booking/FinalizeBooking?villaId?{booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
                    };

                    options.LineItems.Add(new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(booking.TotalCost * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = villa.Name,
                                // Images = new List<string> { domain+ villa.ImageURL},
                            }
                        },
                        Quantity = 1
                    });

                    var service = new SessionService();
                    Session session = service.Create(options);

                    _bookingRepository.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);

                    var DBUpdated = await _unitOfWork.CompleteAsync();

                    Response.Headers.Add("Location", session.Url);
                    TempData["success"] = "Booked successfully";
                    return new StatusCodeResult(303);
                    #endregion
                    //return RedirectToAction(nameof(BookingConfirmation), new { bookingId = booking.Id });
                }

                return BadRequest();
            }
            else
            {
                TempData["error"] = "Booking failed";
                return View(booking);
            }

        }

        [HttpGet]
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            var bookingFromDB = await _bookingRepository.GetAsync(b => b.Id == bookingId, includeProperties: "User,Villa");

            if (bookingFromDB.Status == SD.StatusPending)
            {
                // this is a pending order, we need to confirm if payment was successful

                var service = new SessionService();
                Session session = service.Get(bookingFromDB.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _bookingRepository.UpdateStatus(bookingFromDB.Id, SD.StatusApproved, 0);
                    _bookingRepository.UpdateStripePaymentID(bookingFromDB.Id, session.Id, session.PaymentIntentId);
                    var result = await _unitOfWork.CompleteAsync();

                }
            }
            TempData["success"] = "Payment successful";
            return View(bookingId);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            Booking? booking = await _bookingRepository.GetAsync(booking => booking.Id == bookingId, includeProperties: "Villa,User");
            if (booking is null)
                return NotFound();


            if (booking.VillaNumber == 0 && booking.Status == SD.StatusApproved)
            {
                var availableVillaNumber = await AssignAvailableVillaNumberByVilla(booking.VillaId);

                booking.VillaNumbers = await _villaNumberRepository.GetAllAsync(u => u.VillaId == booking.VillaId
                                                                               &&
                                                                               availableVillaNumber.Any(x => x == u.Villa_Number));
            }
            //var mappedBooking = _mapper.Map<BookingViewModel>(booking);
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _bookingRepository.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            _unitOfWork.CompleteAsync();
            TempData["success"] = "Booking Updated successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _bookingRepository.UpdateStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);
            _unitOfWork.CompleteAsync();
            TempData["success"] = "Booking Completed successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _bookingRepository.UpdateStatus(booking.Id, SD.StatusCancelled, 0);
            _unitOfWork.CompleteAsync();
            TempData["success"] = "Booking Cancelled successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpGet]
        private async Task<List<int>> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            var villaNumbers = await _villaNumberRepository.GetAllAsync(vn => vn.VillaId == villaId);

            var checkedInBookingsForSpecificVilla = await _bookingRepository.GetAllAsync(booking => booking.VillaId == villaId && booking.Status == SD.StatusCheckedIn);
            var checkedInVilla = checkedInBookingsForSpecificVilla.Select(u => u.VillaNumber);

            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return availableVillaNumbers;
        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<Booking> bookings;
            if (User.IsInRole(SD.Role_Admin))
            {
                bookings = await _bookingRepository.GetAllAsync(booking => booking.Status.ToLower().Equals(status.ToLower()), includeProperties: "User,Villa");
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                bookings = await _bookingRepository.GetAllAsync(booking => booking.UserId == user.Id && booking.Status.ToLower().Equals(status.ToLower()), "User,Villa");
            }

            return Json(new { data = bookings });
        }
        #endregion
    }
}
