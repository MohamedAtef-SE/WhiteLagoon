using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IBookingServices
    {
        Task<StatusCodeResult> FinalizeBooking(Booking booking, HttpRequest request, HttpResponse response);
        Task<int?> BookingConfirmation(int bookingId);
        Task<Booking?> BookingDetails(int bookingId);
        Task<int?> CheckInAsync(Booking booking);
        Task<int?> CheckOutAsync(Booking booking);
        Task<int?> CancelAsync(Booking booking);
        Task<IEnumerable<Booking>> GetAllAsync(string status, ClaimsPrincipal User);
        Task<IEnumerable<Booking>> GetAllAsync(Expression<Func<Booking, bool>> filter);
    }
}
