using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Dashboard;
using Services.ApiModels.Dashboard;
using System.Threading.Tasks;

namespace Services.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
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
}
