using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application._Common.Utility;
using WhiteLagoon.Application.DTOs;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class DashboardServices : IDashboardServices
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Booking> _bookingRepository;
        static int previousMonth = DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1;
        private readonly DateTime previousMonthStartDate = new(DateTime.UtcNow.Year, previousMonth, 1);
        private readonly DateTime currentMonthStartDate = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        public DashboardServices(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _bookingRepository = _unitOfWork.GetGenericRepository<Booking>();
        }

        public async Task<RadialBarChartDTO?> GetTotalBookingRadialChartData()
        {
            var totalBookings = await _bookingRepository.GetAll(booking => booking.Status != SD.StatusPending || booking.Status != SD.StatusCancelled).ToListAsync();

            var countByCurrentMonth = totalBookings.Count(booking => booking.BookingDate >= currentMonthStartDate && booking.BookingDate <= DateTime.UtcNow);

            var countByPreviousMonth = totalBookings.Count(booking => booking.BookingDate >= previousMonthStartDate && booking.BookingDate < currentMonthStartDate);

            var radialBarChartDTO = GetRadialBarChartModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);

            return radialBarChartDTO;
        }

        public async Task<RadialBarChartDTO?> GetRegisteredUserChartData()
        {
            var totalUsers = await _userManager.Users.ToListAsync();
            var countByCurrentMonth = totalUsers.Count(user => user.CreatedAt >= currentMonthStartDate && user.CreatedAt <= DateTime.UtcNow);

            var countByPreviousMonth = totalUsers.Count(user => user.CreatedAt >= previousMonthStartDate && user.CreatedAt < currentMonthStartDate);

            var radialBarChartDTO = GetRadialBarChartModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);

            return radialBarChartDTO;
        }

        public async Task<RadialBarChartDTO?> GetRevenueChartData()
        {
            var totalBookings = await _bookingRepository.GetAll(booking => booking.Status != SD.StatusPending || booking.Status != SD.StatusCancelled).ToListAsync();

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(booking => booking.TotalCost));

            var revenueByCurrentMonth = totalBookings.Where(booking => booking.BookingDate >= currentMonthStartDate
                                                          &&
                                                          booking.BookingDate <= DateTime.UtcNow).Sum(booking => booking.TotalCost);

            var revenueByPreviousMonth = totalBookings.Where(booking => booking.BookingDate >= previousMonthStartDate
                                                           &&
                                                           booking.BookingDate < currentMonthStartDate).Sum(booking => booking.TotalCost);

            var radialBarChartDTO = GetRadialBarChartModel(totalRevenue, revenueByCurrentMonth, revenueByPreviousMonth);

            return radialBarChartDTO;
        }

        public async Task<PieChartDTO?> GetBookingPieChartData()
        {
            var totalBookings = await _bookingRepository.GetAll(booking => booking.BookingDate >= DateTime.UtcNow.AddDays(-30)
                                                               &&
                                                              (booking.Status != SD.StatusPending || booking.Status != SD.StatusCancelled))
                                                              .ToListAsync();

            var customersWithOneBooking = totalBookings.GroupBy(booking => booking.UserId).Where(BG => BG.Count() == 1).Select(BG => BG.Key).ToList(); // UserIds

            int bookingsByNewCustomer = customersWithOneBooking.Count();
            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartDTO pieChartDTO = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return pieChartDTO;
        }

        public async Task<LineChartDTO?> GetMemberAndBookingLineChartData()
        {
            var bookingData = await _bookingRepository.GetAll(booking => booking.BookingDate >= DateTime.UtcNow.AddDays(-30))
                                                .GroupBy(booking => booking.BookingDate.Date)
                                                .Select(GBooking => new { Date = GBooking.Key.Date, DailyBookingsCount = GBooking.Count() })
                                                .ToListAsync();

            var customerData = await _userManager.Users.Where(user => user.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                                                 .GroupBy(user => user.CreatedAt.Date)
                                                 .Select(GUser => new { Date = GUser.Key.Date, NewUsersCount = GUser.Count() })
                                                 .ToListAsync();


            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.Date, customer => customer.Date, (booking, customer) =>
            {
                return new
                {
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

            var lineChartDTO = new LineChartDTO()
            {
                Categories = categories,
                Series = new List<ChartData> { bookingsChartData, membersChartData }
            };
            return lineChartDTO;
        }

        private static RadialBarChartDTO GetRadialBarChartModel(int totalCount, double currentMonth, double previousMonth)
        {
            RadialBarChartDTO radialBarChartDTO = new();

            int increaseDecreaseRatio = 100;

            if (previousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonth - previousMonth) / previousMonth * 100);
            }
            radialBarChartDTO.TotalCount = totalCount;
            radialBarChartDTO.CountInCurrentMonth = Convert.ToInt32(currentMonth);
            radialBarChartDTO.HasRatioIncreased = currentMonth > previousMonth;
            radialBarChartDTO.Series = new[] { increaseDecreaseRatio };

            return radialBarChartDTO;
        }
    }
}
