using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Domain.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        void UpdateStatus(int bookingId, string orderStatus,int villaNumber = 0);
        void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
    }
}
