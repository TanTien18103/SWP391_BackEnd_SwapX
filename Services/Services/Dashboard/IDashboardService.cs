using Services.ApiModels;
using Services.ApiModels.Dashboard;
using System.Threading.Tasks;

namespace Services.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync();
    Task<ResultModel> ShowDashboard();
}

