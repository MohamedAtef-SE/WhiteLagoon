using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteLagoon.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "looks, you missed your name")]
        [MaxLength(50)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Price per night")]
        [Range(100,10000)]
        public double Price { get; set; }
        public int Sqft { get; set; }
        [Range(1,10)]
        public int Occupancy { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageURL { get; set; }
        public virtual IEnumerable<Amenity>? Amenities { get; set; } = new HashSet<Amenity>();
        [NotMapped]
        public bool IsAvailable { get; set; } = true;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
