using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        public int VillaId { get; set; }

        [ForeignKey(nameof(VillaId))]
        public virtual Villa Villa { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        [Required]
        public double TotalCost { get; set; }
        public int Nights { get; set; }

        [Required]
        public string? Status { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public DateOnly CheckInDate { get; set; }
        [Required]
        public DateOnly CheckOutDate { get; set; }

        public bool IsPaymentSuccessful { get; set; } = false;
        public DateTime PaymentDate { get; set; }

        public string? StripeSessionId { get; set; }
        public string? StripePaymentInentId { get; set; }
        public DateTime ActualCheckInDate {  get; set; }
        public DateTime ActualCheckOutDate { get;set; }
        public int VillaNumber { get; set; }
        
        [NotMapped]
        public IEnumerable<VillaNumber> VillaNumbers { get; set; } = new List<VillaNumber>();
    }
}
