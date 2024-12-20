namespace WhiteLagoon.Web.ViewModels
{
    public class RadialBarChartVM
    {
        public decimal TotalCount { get; set; }
        public int CountInCurrentMonth { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series {  get; set; }
    }
}
