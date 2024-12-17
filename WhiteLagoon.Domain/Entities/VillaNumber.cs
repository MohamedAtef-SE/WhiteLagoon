using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteLagoon.Domain.Entities
{
    public class VillaNumber
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Villa Number")]
        
        public int Villa_Number { get; set; }
        [Display(Name = "Villa ID")]

        [ForeignKey("Villa")]
        public int VillaId { get; set; } // Without Unique DataAnnotation on VillaId it makes Navigational Property Many
        [ValidateNever]
        public Villa Villa { get; set; } = null!;
        [Display(Name = "Special Details")]
        public string? SpecialDetails { get; set; }
    }
}
