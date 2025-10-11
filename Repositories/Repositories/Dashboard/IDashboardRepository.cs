using System.Threading.Tasks;

namespace Repositories.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        Task<int> CountAccountsAsync();
        Task<int> CountOrdersAsync();
        Task<int> CountBatteriesAsync();
        Task<int> CountExchangeBatteriesAsync();
        Task<int> CountStationsAsync();
        Task<int> CountPackagesAsync();
        Task<int> CountFormsAsync();
        Task<int> CountRatingsAsync();
        Task<int> CountReportsAsync();
        Task<int> CountSlotsAsync();
        Task<int> CountVehiclesAsync();
        Task<int> CountBatteryReportsAsync();
        Task<int> CountStationSchedulesAsync();
    }
}

