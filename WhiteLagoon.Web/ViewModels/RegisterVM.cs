using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.Web.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [EmailAddress(ErrorMessage ="Invalid email address")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(20)]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Display(Name ="Phone Number")]
        [Phone]
        //[RegularExpression("^(\\+20|0020)?01[0125]\\d{7}$\r\n",ErrorMessage ="invalid phone number")]
        public string? PhoneNumber { get; set; }
        public string? Role {  get; set; }
        [ValidateNever]
        public string? ReturnUrl { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
