using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        void UpdateStatus(int bookingId, string orderStatus,int villaNumber);
        void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
    }
}
