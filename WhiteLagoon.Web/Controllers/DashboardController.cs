using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Booking> _bookingRepository;
        static int previousMonth = DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1;
        private readonly DateTime previousMonthStartDate = new(DateTime.UtcNow.Year, previousMonth, 1);
        private readonly DateTime currentMonthStartDate = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _bookingRepository = _unitOfWork.GetGenericRepository<Booking>();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var totalBookings = await _bookingRepository.GetAll(booking => booking.Status != SD.StatusPending || booking.Status != SD.StatusCancelled).ToListAsync();

            var countByCurrentMonth = totalBookings.Count(booking => booking.BookingDate >= currentMonthStartDate && booking.BookingDate <= DateTime.UtcNow);

            var countByPreviousMonth = totalBookings.Count(booking => booking.BookingDate >= previousMonthStartDate && booking.BookingDate < currentMonthStartDate);

            var radialBarChartVM = GetRadialBarChartModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);

            return Json(radialBarChartVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            var totalUsers = await _userManager.Users.ToListAsync();
            var countByCurrentMonth = totalUsers.Count(user => user.CreatedAt >= currentMonthStartDate && user.CreatedAt <= DateTime.UtcNow);

            var countByPreviousMonth = totalUsers.Count(user => user.CreatedAt >= previousMonthStartDate && user.CreatedAt < currentMonthStartDate);

            var radialBarChartVM = GetRadialBarChartModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);

            return Json(radialBarChartVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueChartData()
        {
            var totalBookings = await _bookingRepository.GetAll(booking => booking.Status != SD.StatusPending || booking.Status != SD.StatusCancelled).ToListAsync();

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(booking => booking.TotalCost));

            var revenueByCurrentMonth = totalBookings.Where(booking => booking.BookingDate >= currentMonthStartDate
                                                          &&
                                                          booking.BookingDate <= DateTime.UtcNow).Sum(booking => booking.TotalCost);

            var revenueByPreviousMonth = totalBookings.Where(booking => booking.BookingDate >= previousMonthStartDate
                                                           &&
                                                           booking.BookingDate < currentMonthStartDate).Sum(booking => booking.TotalCost);

            var radialBarChartVM = GetRadialBarChartModel(totalRevenue, revenueByCurrentMonth, revenueByPreviousMonth);

            return Json(radialBarChartVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingPieChartData()
        {
            var totalBookings = await _bookingRepository.GetAll(booking => booking.BookingDate >= DateTime.UtcNow.AddDays(-30)
                                                                &&
                                                               (booking.Status != SD.StatusPending || booking.Status != SD.StatusCancelled))
                                                               .ToListAsync();

            var customersWithOneBooking = totalBookings.GroupBy(booking => booking.UserId).Where(BG => BG.Count() == 1).Select(BG => BG.Key).ToList(); // UserIds

            int bookingsByNewCustomer = customersWithOneBooking.Count();
            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartVM pieChartVM = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return Json(pieChartVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingData = await _bookingRepository.GetAll(booking => booking.BookingDate >= DateTime.UtcNow.AddDays(-30))
                                                .GroupBy(booking => booking.BookingDate.Date)
                                                .Select(GBooking => new { Date = GBooking.Key.Date, DailyBookingsCount = GBooking.Count() })
                                                .ToListAsync();

            var customerData = await _userManager.Users.Where(user => user.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                                                 .GroupBy(user => user.CreatedAt.Date)
                                                 .Select(GUser => new { Date = GUser.Key.Date, NewUsersCount = GUser.Count() })
                                                 .ToListAsync();


            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.Date, customer => customer.Date, (booking,customer) =>
            {
                return new {
                    booking.Date,
                    booking.DailyBookingsCount,
                    NewUsersCount = customer.Select(c => c.NewUsersCount).FirstOrDefault()
                };
            });

            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.Date, booking => booking.Date, (customer, booking) =>
            {
                return new
                {
                    customer.Date,
                    DailyBookingsCount = booking.Select(booking => booking.DailyBookingsCount).FirstOrDefault(),
                    customer.NewUsersCount,
                };
            });
            var mergedData = leftJoin.Union(rightJoin).OrderBy(m => m.Date).ToList();

            var categories = mergedData.Select(m => m.Date.ToString("MM/dd/yyyy")).ToArray();
            var bookedData = mergedData.Select(m => m.DailyBookingsCount).ToArray();
            var membersData = mergedData.Select(m => m.NewUsersCount).ToArray(); 

            var bookingsChartData = new ChartData()
            {
                Name = "New Bookings",
                Data = bookedData
            };

            var membersChartData = new ChartData()
            {
                Name = "New Customers",
                Data = membersData
            };

            var lineChartVM = new LineChartVM()
            {
                Categories = categories,
                Series = new List<ChartData> { bookingsChartData, membersChartData }
            };
            return Json(lineChartVM);

        }

        private static RadialBarChartVM GetRadialBarChartModel(int totalCount, double currentMonth, double previousMonth)
        {
            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;

            if (previousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonth - previousMonth) / previousMonth * 100);
            }
            radialBarChartVM.TotalCount = totalCount;
            radialBarChartVM.CountInCurrentMonth = Convert.ToInt32(currentMonth);
            radialBarChartVM.HasRatioIncreased = currentMonth > previousMonth;
            radialBarChartVM.Series = new[] { increaseDecreaseRatio };

            return radialBarChartVM;
        }
    }
}
