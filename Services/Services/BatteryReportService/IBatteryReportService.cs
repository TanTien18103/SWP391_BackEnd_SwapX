using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.BatteryReport;
using Services.ApiModels;

namespace Services.Services.BatteryReportService
{
    public interface IBatteryReportService
    {
        Task<ResultModel> GetBatteryReportById(string batteryReportId);
        Task<ResultModel> GetAllBatteryReports();
        Task<ResultModel> AddBatteryReport(AddBatteryReportRequest addBatteryReportRequest);
        Task<ResultModel> AddBatteryReportDirect(AddBatteryReportDirectRequest addBatteryReportDirectRequest);
        Task<ResultModel> UpdateBatteryReport(UpdateBatteryReportRequest updateBatteryReportRequest);
        Task<ResultModel> DeleteBatteryReport(string batteryReportId);
        Task<ResultModel> GetBatteryReportsByStation(string stationId);
        Task<ResultModel> GetBatteryReportsByBatteryId(string batteryId);
    }
}
