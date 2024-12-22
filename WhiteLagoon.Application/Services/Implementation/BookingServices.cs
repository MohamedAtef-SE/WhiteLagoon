using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Linq.Expressions;
using System.Security.Claims;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class BookingServices : IBookingServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceManager _serviceManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingRepository _bookingRepository;

        public BookingServices(IServiceManager serviceManager, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IBookingRepository bookingRepository)
        {
            _serviceManager = serviceManager;
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync(string status, ClaimsPrincipal User)
        {
            IEnumerable<Booking> bookings;
            if (User.IsInRole(SD.Role_Admin))
            {
                bookings = await _bookingRepository.GetAll(booking => booking.Status!.ToLower().Equals(status.ToLower()), includeProperties: "User,Villa").ToListAsync();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                bookings = await _bookingRepository.GetAll(booking => booking.UserId == user!.Id && booking.Status!.ToLower().Equals(status.ToLower()), "User,Villa").ToListAsync();
            }
            return bookings;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync(Expression<Func<Booking, bool>> filter)
        {
            return await _bookingRepository.GetAll(filter).ToListAsync();
        }

        public async Task<StatusCodeResult> FinalizeBooking(Booking booking, HttpRequest request, HttpResponse response)
        {
            var villa = await _serviceManager.VillaServices.GetAsync(booking.VillaId);
            if (villa is null) throw new Exception("villa not found");

            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.UtcNow;

            var villasNumber = await _serviceManager.VillaNumberServices.GetAllAsync();
            var bookedVillas = await _bookingRepository.GetAll(booking => booking.Status == SD.StatusApproved
                                                                            || booking.Status == SD.StatusCheckedIn).ToListAsync();


            int roomsAvailable = SD.VillaRoomsAvailable_Count((int)villa.Id, villasNumber.ToList(),
                                                              booking.CheckInDate, booking.Nights,
                                                              bookedVillas.ToList());

            if (roomsAvailable == 0)
            {
                return new StatusCodeResult(404);
            }

            var result = await _bookingRepository.AddAsync(booking);

            if (result)
            {
                var completed = await _unitOfWork.CompleteAsync();
                if (completed)
                {
                    #region Stripe

                    var domain = $"{request.Scheme}://{request.Host.Value}";

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

                    response.Headers.Add("Location", session.Url);

                    return new StatusCodeResult(303);
                    #endregion
                }

                return new StatusCodeResult(400);
            }
            else
            {
                return new StatusCodeResult(400);
            }
        }

        public async Task<int?> BookingConfirmation(int bookingId)
        {
            var booking = await _bookingRepository.GetAsync(b => b.Id == bookingId, includeProperties: "User,Villa");
            if (booking is null)
                return null;
            if (booking.Status == SD.StatusPending)
            {
                // this is a pending order, we need to confirm if payment was successful

                var service = new SessionService();
                Session session = service.Get(booking.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _bookingRepository.UpdateStatus(booking.Id, SD.StatusApproved, 0);
                    _bookingRepository.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
                    var result = await _unitOfWork.CompleteAsync();
                    if (!result) return null;
                }
            }

            return bookingId;
        }

        public async Task<Booking?> BookingDetails(int bookingId)
        {
            Booking? booking = await _bookingRepository.GetAsync(booking => booking.Id == bookingId, includeProperties: "Villa,User");
            if (booking is null)
                return null;


            if (booking.VillaNumber == 0 && booking.Status == SD.StatusApproved)
            {
                var availableVillaNumber = await AssignAvailableVillaNumberByVilla(booking.VillaId);

                booking.VillaNumbers = await _serviceManager.VillaNumberServices.GetAllAsync(u => u.VillaId == booking.VillaId
                                                                               &&
                                                                               availableVillaNumber.Any(x => x == u.Villa_Number));
            }

            return booking;
        }

        public async Task<int?> CheckInAsync(Booking booking)
        {
            _bookingRepository.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            var result = await _unitOfWork.CompleteAsync();
            if (!result) return null;

            return booking.Id;
        }

        public async Task<int?> CheckOutAsync(Booking booking)
        {
            _bookingRepository.UpdateStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);
            var result = await _unitOfWork.CompleteAsync();
            if (!result) return null;

            return booking.Id;
        }

        public async Task<int?> CancelAsync(Booking booking)
        {
            _bookingRepository.UpdateStatus(booking.Id, SD.StatusCancelled, booking.VillaNumber);
            var result = await _unitOfWork.CompleteAsync();
            if (!result) return null;

            return booking.Id;
        }

        private async Task<List<int>> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            var villaNumbers = await _serviceManager.VillaNumberServices.GetAllAsync();

            var checkedInBookingsForSpecificVilla = await _bookingRepository.GetAll(booking => booking.VillaId == villaId
                                                                                    &&
                                                                                    booking.Status == SD.StatusCheckedIn).ToListAsync();

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

    }
}
