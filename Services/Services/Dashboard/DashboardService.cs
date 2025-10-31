using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.Dashboard;
using Repositories.Repositories.ExchangeBatteryRepo;
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
    private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
    public DashboardService(
        IDashboardRepository dashboardRepository,
        IOrderRepository orderRepository,
        IAccountRepo accountRepo,
        IBatteryRepo batteryRepository,
        IStationRepo stationRepository,
        IExchangeBatteryRepo exchangeBatteryRepo
        )
    {
        _dashboardRepository = dashboardRepository;
        _orderRepository = orderRepository;
        _accountRepo = accountRepo;
        _batteryRepository = batteryRepository;
        _stationRepository = stationRepository;
        _exchangeBatteryRepo = exchangeBatteryRepo;
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

    public async Task<ResultModel> ShowDashboard(DashboardFilterRequest filter)
    {
        try
        {
            var startDate = filter.StartDate ?? TimeHepler.SystemTimeNow.AddYears(-100);
            var endDate = filter.EndDate ?? TimeHepler.SystemTimeNow;
            var stationId = filter.StationId;

            var stations = await _stationRepository.GetAllStations();
            var users = await _accountRepo.GetAll();
            var batteries = await _batteryRepository.GetAllBatteries();
            var orders = await _orderRepository.GetAllOrdersAsync();
            var exchanges = await _exchangeBatteryRepo.GetAll();

            var paidOrders = orders
                .Where(o => o.Status == PaymentStatus.Paid.ToString())
                .ToList();

            var completedExchange = exchanges
                .Where(e => e.Status.Equals(ExchangeStatusEnums.Completed.ToString(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            var completedOrders = (from o in paidOrders
                                   join e in completedExchange on o.OrderId equals e.OrderId
                                   select new
                                   {
                                       Order = o,
                                       Exchange = e
                                   }).ToList();


            if (!string.IsNullOrEmpty(stationId))
            {
                completedOrders = completedOrders
                    .Where(x => x.Exchange.StationId == stationId)
                    .ToList();
            }

            completedOrders = completedOrders
                .Where(x => x.Order.StartDate >= startDate && x.Order.StartDate <= endDate)
                .ToList();

            var totalRevenue = completedOrders.Sum(x => x.Order.Total ?? 0);
            var totalOrders = completedOrders.Count;
            var totalUsers = users.Count;
            var totalStations = stations.Count;
            var totalBatteries = batteries.Count;

            var revenueOverTime = paidOrders
                .GroupBy(x => x.StartDate.Value.Date)
                .Select(g => new RevenueChartItem
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.Total ?? 0)
                })
                .OrderBy(x => x.Date)
                .ToList();

            var userGrowthChart = users
                .GroupBy(x => x.StartDate.Value.Date)
                .Select(g => new UserGrowthItem
                {
                    Date = g.Key,
                    NewUsers = g.Count(),
                    TotalUsers = users.Count(u => u.StartDate <= g.Key)
                })
                .OrderBy(x => x.Date)
                .ToList();

            var topStations = completedOrders
                .GroupBy(x => x.Exchange.StationId)
                .Select(g => new TopStationItem
                {
                    StationId = g.Key,
                    StationName = stations.FirstOrDefault(s => s.StationId == g.Key)?.StationName ?? RoleEnums.Unknown.ToString(),
                    Revenue = g.Sum(t => t.Order.Total ?? 0),
                    Totalorders = g.Count()
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            var revenueByServiceType = paidOrders
                .GroupBy(x => x.ServiceType)
                .Select(g => new RevenueByServiceTypeItem
                {
                    ServiceType = g.Key,
                    Revenue = g.Sum(t => t.Total ?? 0),
                    Orders = g.Count()
                })
                .ToList();

            var batteryStatusSummary = new BatteryStatusSummary
            {
                Total = batteries.Count,
                Charging = batteries.Count(b => b.Status == BatteryStatusEnums.Charging.ToString()),
                InUse = batteries.Count(b => b.Status == BatteryStatusEnums.InUse.ToString()),
                Available = batteries.Count(b => b.Status == BatteryStatusEnums.Available.ToString()),
                Decommissioned = batteries.Count(b => b.Status == BatteryStatusEnums.Decommissioned.ToString()),
                Booked = batteries.Count(b => b.Status == BatteryStatusEnums.Booked.ToString()),
                Maintenance = batteries.Count(b => b.Status == BatteryStatusEnums.Maintenance.ToString())
            };

            var response = new ShowDashboardResponse
            {
                TotalStations = totalStations,
                TotalUsers = totalUsers,
                TotalBatteries = totalBatteries,
                Totalorders = totalOrders,
                TotalRevenue = totalRevenue,
                RevenueOverTime = revenueOverTime,
                UserGrowthChart = userGrowthChart,
                TopStations = topStations,
                RevenueByServiceType = revenueByServiceType,
                BatteryStatusSummary = batteryStatusSummary,
                FilterInfo = new DashboardFilterResponse
                {
                    SelectedStationId = stationId,
                    SelectedStationName = stations.FirstOrDefault(s => s.StationId == stationId)?.StationName,
                    StartDate = startDate,
                    EndDate = endDate
                }
            };

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ResponseMessageConstantsDashboard.DASHBOARD_LOADED_SUCCESS,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ex.Message
            };
        }
    }
}

