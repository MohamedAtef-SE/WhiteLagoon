namespace WhiteLagoon.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string? ImageURL { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateData { get; set; }
    }
}
