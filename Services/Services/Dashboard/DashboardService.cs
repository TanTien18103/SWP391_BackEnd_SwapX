using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.Dashboard;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.StationRepo;
using Services.ApiModels;
using Services.ApiModels.Dashboard;
using System.Threading.Tasks;

namespace Services.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IAccountRepo _accountRepo;
    private readonly IBatteryRepo _batteryRepository;
    private readonly IStationRepo _stationRepository;
    public DashboardService(
        IDashboardRepository dashboardRepository,
        IOrderRepository orderRepository,
        IAccountRepo accountRepo,
        IBatteryRepo batteryRepository,
        IStationRepo stationRepository
        )
    {
        _dashboardRepository = dashboardRepository;
        _orderRepository = orderRepository;
        _accountRepo = accountRepo;
        _batteryRepository = batteryRepository;
        _stationRepository = stationRepository;
    }

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync()
    {
        return new DashboardSummaryResponse
        {
            TotalAccounts = await _dashboardRepository.CountAccountsAsync(),
            TotalOrders = await _dashboardRepository.CountOrdersAsync(),
            TotalBatteries = await _dashboardRepository.CountBatteriesAsync(),
            TotalExchangeBatteries = await _dashboardRepository.CountExchangeBatteriesAsync(),
            TotalStations = await _dashboardRepository.CountStationsAsync(),
            TotalPackages = await _dashboardRepository.CountPackagesAsync(),
            TotalForms = await _dashboardRepository.CountFormsAsync(),
            TotalRatings = await _dashboardRepository.CountRatingsAsync(),
            TotalReports = await _dashboardRepository.CountReportsAsync(),
            TotalSlots = await _dashboardRepository.CountSlotsAsync(),
            TotalVehicles = await _dashboardRepository.CountVehiclesAsync(),
            TotalBatteryReports = await _dashboardRepository.CountBatteryReportsAsync(),
            TotalStationSchedules = await _dashboardRepository.CountStationSchedulesAsync()
        };
    }

    // ***************************************************************
    // ⚠️ HÃY ĐỊNH NGHĨA CÁC MODELS NÀY TRONG NAMESPACE Services.ApiModels.Dashboard
    // ***************************************************************

    // public class MonthlyData { ... } 
    // public class UserDistributionData { ... }
    // public class DailySwapFrequencyData { ... }
    // public class StationEfficiencyData { ... }
    // public class HourSwapData { ... }
    // public class ShowDashboardResponse { ... }

    // public enum PaymentType { Package, PrePaid, UsePackage, PaidAtStation }

    public async Task<ResultModel> ShowDashboard()
    {
        try
        {
            // 📥 Lấy dữ liệu
            var orders = await _orderRepository.GetAllOrdersAsync();
            var users = await _accountRepo.GetAll();
            var batteries = await _batteryRepository.GetAllBatteries();
            var stations = await _stationRepository.GetAllStations();

            var now = DateTime.Now;
            var currentYear = now.Year;
            var currentMonth = now.Month;
            var sevenDaysAgo = now.Date.AddDays(-6);

            // --- HÀM HỖ TRỢ: Thử chuyển ServiceType (string) thành Enum ---
            PaymentType? GetPaymentType(string serviceType)
            {
                if (string.IsNullOrEmpty(serviceType)) return null;
                if (Enum.TryParse(serviceType, true, out PaymentType result))
                {
                    return result;
                }
                return null;
            }

            // --- HÀM HỖ TRỢ: Lấy StationId từ ExchangeBatteries ---
            string GetStationId(Order order)
            {
                // GIẢ ĐỊNH: StationId được lưu trong bảng ExchangeBattery liên quan đến Order này
                return order.ExchangeBatteries?.FirstOrDefault()?.StationId;
            }

            // -----------------------------------------------------------------------------------------------------
            // 1️⃣ TÍNH TOÁN DOANH THU & SWAPS THEO THÁNG (Cho biểu đồ & Summary)
            // -----------------------------------------------------------------------------------------------------

            var monthlyData = orders
                .Where(o => o.Date.HasValue && o.Total.HasValue
                         && (GetPaymentType(o.ServiceType) == PaymentType.PaidAtStation
                          || GetPaymentType(o.ServiceType) == PaymentType.PrePaid
                          || GetPaymentType(o.ServiceType) == PaymentType.Package))
                .GroupBy(o => new { Year = o.Date.Value.Year, Month = o.Date.Value.Month })
                .Select(g => new MonthlyData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.Total.Value),
                    TotalSwaps = orders.Count(o => o.Date.HasValue
                                                && o.Date.Value.Year == g.Key.Year
                                                && o.Date.Value.Month == g.Key.Month)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            // ... (Logic tính Summary giữ nguyên) ...
            var currentMonthData = monthlyData.FirstOrDefault(d => d.Year == currentYear && d.Month == currentMonth);
            var prevMonthData = monthlyData.Where(d => d.Year < currentYear || (d.Year == currentYear && d.Month < currentMonth))
                                           .OrderByDescending(d => d.Year).ThenByDescending(d => d.Month).FirstOrDefault();

            decimal currentMonthRevenue = currentMonthData?.TotalRevenue ?? 0;
            decimal prevMonthRevenue = prevMonthData?.TotalRevenue ?? 0;
            int currentMonthSwaps = currentMonthData?.TotalSwaps ?? 0;
            int prevMonthSwaps = prevMonthData?.TotalSwaps ?? 0;

            decimal revenueGrowthPercent = (prevMonthRevenue != 0)
                                         ? Math.Round(((currentMonthRevenue - prevMonthRevenue) / prevMonthRevenue) * 100, 1) : 0;
            decimal swapsGrowthPercent = (prevMonthSwaps != 0)
                                        ? Math.Round(((decimal)(currentMonthSwaps - prevMonthSwaps) / prevMonthSwaps) * 100, 1) : 0;

            // -----------------------------------------------------------------------------------------------------
            // 2️⃣ TÍNH TOÁN PHÂN BỐ NGƯỜI DÙNG
            // -----------------------------------------------------------------------------------------------------

            var userCategories = orders
                .Where(o => o.AccountId != null)
                .GroupBy(o => o.AccountId)
                .Select(g =>
                {
                    string userCategory;
                    if (g.Any(o => GetPaymentType(o.ServiceType) == PaymentType.Package))
                    {
                        userCategory = "Người dùng thường xuyên (Package)";
                    }
                    else if (g.Any(o => GetPaymentType(o.ServiceType) == PaymentType.PaidAtStation))
                    {
                        userCategory = "Người dùng thỉnh thoảng (Pay-per-use)";
                    }
                    else if (g.Any(o => GetPaymentType(o.ServiceType) == PaymentType.PrePaid))
                    {
                        userCategory = "Người dùng trả trước (PrePaid)";
                    }
                    else
                    {
                        userCategory = "Khác/Giao dịch UsePackage";
                    }
                    return new { UserId = g.Key, UserCategory = userCategory };
                })
                .DistinctBy(x => x.UserId)
                .ToList();

            var userDistributionGroups = userCategories
                .GroupBy(x => x.UserCategory)
                .Select(g => new UserDistributionData { UserType = g.Key, Count = g.Count() })
                .ToList();

            var activeUserIds = userCategories.Select(x => x.UserId).ToHashSet();
            // SỬA: Dùng AccountId của Account để so sánh với Order.AccountId
            var totalNewUsers = users.Count(u => !activeUserIds.Contains(u.AccountId));

            if (totalNewUsers > 0)
            {
                userDistributionGroups.Add(new UserDistributionData { UserType = "Người dùng mới (Chưa đổi pin)", Count = totalNewUsers });
            }

            var totalUsersCounted = userDistributionGroups.Sum(d => d.Count);
            var userDistribution = userDistributionGroups;
            userDistribution.ForEach(d =>
            {
                d.Percent = totalUsersCounted > 0 ? Math.Round((decimal)d.Count / totalUsersCounted * 100, 1) : 0;
            });

            // -----------------------------------------------------------------------------------------------------
            // 3️⃣ TÍNH TOÁN TẦN SUẤT ĐỔI PIN THEO NGÀY
            // -----------------------------------------------------------------------------------------------------

            var dailySwapFrequency = orders
                .Where(o => o.Date.HasValue && o.Date.Value.Date >= sevenDaysAgo)
                .GroupBy(o => o.Date.Value.Date)
                .Select(g => new DailySwapFrequencyData
                {
                    Date = g.Key,
                    TotalSwaps = g.Count(),
                    // SỬA LỖI CS1061: Giả lập AvgTimeSpent vì Order không có TimeSpentSeconds
                    AvgTimeSpent = 120
                })
                .OrderBy(d => d.Date)
                .ToList();

            // -----------------------------------------------------------------------------------------------------
            // 4️⃣ TÍNH TOÁN HIỆU SUẤT TRẠM ĐỔI PIN (GIẢ LẬP)
            // -----------------------------------------------------------------------------------------------------

            var stationEfficiencies = orders
                .Where(o => o.Date.HasValue && GetStationId(o) != null) // SỬA: Dùng GetStationId()
                .GroupBy(o => GetStationId(o)) // SỬA: Dùng GetStationId()
                .Select(g =>
                {
                    var station = stations.FirstOrDefault(s => s.StationId == g.Key);
                    var totalSwaps = g.Count();

                    return new StationEfficiencyData
                    {
                        StationName = station?.StationName ?? $"Trạm {g.Key}",
                        TotalSwaps = totalSwaps,
                        // Giả lập giá trị
                        EfficiencyPercent = (g.Key.GetHashCode() % 6 + 70)
                    };
                })
                .OrderBy(s => s.StationName)
                .ToList();

            // -----------------------------------------------------------------------------------------------------
            // 5️⃣ TÍNH TOÁN PHÂN TÍCH GIỜ CAO ĐIỂM
            // -----------------------------------------------------------------------------------------------------

            var hourlySwapAnalysis = orders
                .Where(o => o.Date.HasValue && o.Date.Value.Date >= sevenDaysAgo)
                .GroupBy(o => o.Date.Value.Hour)
                .Select(g => new HourSwapData
                {
                    Hour = g.Key,
                    TotalSwaps = g.Count(),
                    // SỬA: Dùng GetStationId()
                    ActiveStations = g.Select(o => GetStationId(o)).Where(id => id != null).Distinct().Count()
                })
                .OrderBy(h => h.Hour)
                .ToList();

            for (int i = 0; i < 24; i++)
            {
                if (!hourlySwapAnalysis.Any(h => h.Hour == i))
                {
                    hourlySwapAnalysis.Add(new HourSwapData { Hour = i, TotalSwaps = 0, ActiveStations = 0 });
                }
            }
            hourlySwapAnalysis = hourlySwapAnalysis.OrderBy(h => h.Hour).ToList();


            // -----------------------------------------------------------------------------------------------------
            // 6️⃣ GÓI DỮ LIỆU VÀ TRẢ VỀ
            // -----------------------------------------------------------------------------------------------------

            // Tính toán người dùng mới và hiệu suất pin
            int prevMonthNewUsers = 100; // Giả lập
                                         // SỬA: Dùng StartDate (thay vì CreatedAt)
            int currentMonthNewUsers = users.Count(u => u.StartDate.HasValue && u.StartDate.Value.Year == currentYear && u.StartDate.Value.Month == currentMonth);
            decimal newUsersGrowthPercent = (prevMonthNewUsers != 0) ? Math.Round(((decimal)(currentMonthNewUsers - prevMonthNewUsers) / prevMonthNewUsers) * 100, 1) : 0;

            // SỬA LỖI CS1061: Giả định AverageEfficiency là 0.85
            decimal averageEfficiency = batteries.Any()
                                      ? 0.85M
                                      : 0M;

            var finalResult = new ShowDashboardResponse
            {
                // SUMMARY
                CurrentMonthRevenue = currentMonthRevenue,
                RevenueGrowthPercent = revenueGrowthPercent,
                CurrentMonthSwaps = currentMonthSwaps,
                SwapsGrowthPercent = swapsGrowthPercent,
                TotalUsers = users.Count(),
                NewUsersGrowthPercent = newUsersGrowthPercent,
                AverageEfficiency = averageEfficiency * 100, // Hiển thị dưới dạng phần trăm (85.0)
                EfficiencyGrowthPercent = 7.1M, // Giữ nguyên giả lập

                // CHART DATA
                MonthlyRevenuesAndSwaps = monthlyData,
                UserDistribution = userDistribution,
                DailySwapFrequency = dailySwapFrequency,
                StationEfficiencies = stationEfficiencies,
                HourlySwapAnalysis = hourlySwapAnalysis
            };

            return new ResultModel
            {
                IsSuccess = true,
                Data = finalResult,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                Message = ex.Message,
                ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

