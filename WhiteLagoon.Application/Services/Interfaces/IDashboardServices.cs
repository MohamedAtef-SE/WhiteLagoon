using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.DTOs;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IDashboardServices
    {
        Task<RadialBarChartDTO?> GetTotalBookingRadialChartData();
        Task<RadialBarChartDTO?> GetRegisteredUserChartData();
        Task<RadialBarChartDTO?> GetRevenueChartData();
        Task<PieChartDTO?> GetBookingPieChartData();
        Task<LineChartDTO?> GetMemberAndBookingLineChartData();


    }
}
