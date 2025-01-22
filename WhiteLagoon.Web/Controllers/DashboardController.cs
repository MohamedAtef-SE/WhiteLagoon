using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services;
using WhiteLagoon.Application.Services.Interfaces;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController(IServiceManager _serviceManager) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var radialBarChartDTO = await _serviceManager.DashboardServices.GetTotalBookingRadialChartData();

            return Json(radialBarChartDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            var radialBarChartDTO = await _serviceManager.DashboardServices.GetRegisteredUserChartData();
            return Json(radialBarChartDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueChartData()
        {
            var radialBarChartDTO = await _serviceManager.DashboardServices.GetRevenueChartData();
            return Json(radialBarChartDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingPieChartData()
        {
            var pieChartDTO = await _serviceManager.DashboardServices.GetBookingPieChartData();

            return Json(pieChartDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var lineChartDTO = await _serviceManager.DashboardServices.GetMemberAndBookingLineChartData();
            return Json(lineChartDTO);

        }

    }
}
