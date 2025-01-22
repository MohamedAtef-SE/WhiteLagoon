using System.ComponentModel.DataAnnotations;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Web.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int VillaId { get; set; }

        public VillaViewModel Villa { get; set; } = null!;

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
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Check_Out_Date { get; set; }
       
        [DataType(DataType.Date)]
        public DateOnly Check_In_Date { get; set; }

        public bool IsPaymentSuccessful { get; set; } = false;
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; }

        public string? StripeSessionId { get; set; }
        public string? StripePaymentInentId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ActualCheckInDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ActualCheckOutDate { get; set; }
        public int VillaNumber { get; set; }
        public List<VillaNumberVM> VillaNumbers { get; set; }
    }
}
