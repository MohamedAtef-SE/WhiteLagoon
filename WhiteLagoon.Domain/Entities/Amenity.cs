using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteLagoon.Domain.Entities
{
    public class Amenity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        [ForeignKey(nameof(Villa))]
        public int? VillaId { get; set; }
        public virtual Villa? Villa { get; set; }
    }
}
