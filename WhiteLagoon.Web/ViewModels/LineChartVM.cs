namespace WhiteLagoon.Web.ViewModels
{
    public class LineChartVM
    {
        public List<ChartData> Series { get; set; } = new List<ChartData>();
        public string[] Categories { get; set; } = null!;
    }

    public class ChartData
    {
        public string Name { get; set; } = null!;
        public int[] Data { get; set; } = null!;
    }
}
