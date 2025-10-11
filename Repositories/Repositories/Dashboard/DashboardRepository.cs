using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repositories.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly SwapXContext _context;
        public DashboardRepository(SwapXContext context)
        {
            _context = context;
        }

        public async Task<int> CountAccountsAsync() => await _context.Accounts.CountAsync();
        public async Task<int> CountOrdersAsync() => await _context.Orders.CountAsync();
        public async Task<int> CountBatteriesAsync() => await _context.Batteries.CountAsync();
        public async Task<int> CountExchangeBatteriesAsync() => await _context.ExchangeBatteries.CountAsync();
        public async Task<int> CountStationsAsync() => await _context.Stations.CountAsync();
        public async Task<int> CountPackagesAsync() => await _context.Packages.CountAsync();
        public async Task<int> CountFormsAsync() => await _context.Forms.CountAsync();
        public async Task<int> CountRatingsAsync() => await _context.Ratings.CountAsync();
        public async Task<int> CountReportsAsync() => await _context.Reports.CountAsync();
        public async Task<int> CountSlotsAsync() => await _context.Slots.CountAsync();
        public async Task<int> CountVehiclesAsync() => await _context.Vehicles.CountAsync();
        public async Task<int> CountBatteryReportsAsync() => await _context.BatteryReports.CountAsync();
        public async Task<int> CountStationSchedulesAsync() => await _context.StationSchedules.CountAsync();
    }
}
