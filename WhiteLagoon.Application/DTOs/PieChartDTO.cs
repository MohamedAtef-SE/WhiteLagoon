namespace WhiteLagoon.Application.DTOs
{
    public class PieChartDTO
    {
        public decimal[] Series { get; set; } = null!;
        public string[] Labels { get; set; } = null!;
    }
}
