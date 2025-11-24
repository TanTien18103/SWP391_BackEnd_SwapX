using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.Battery;
using Services.ApiModels;

namespace Services.Services.BatteryService
{
    public interface IBatteryService
    {
        Task<ResultModel> GetBatteryById(string batteryId);
        Task<ResultModel> AddBattery(AddBatteryRequest addBatteryRequest);
        Task<ResultModel> UpdateBattery(UpdateBatteryRequest updateBatteryRequest);
        Task<ResultModel> GetAllBatteries();
        Task<ResultModel> GetBatteriesByStationId(string stationId);
        Task<ResultModel> DeleteBattery(string batteryId);
        Task<ResultModel> AddBatteryInStation(AddBatteryInStationRequest addBatteryInStationRequest);
        Task<ResultModel> UpdateBatteryStatusInStation(UpdateBatteryStatusRequest updateBatteryStatusRequest);
        Task<ResultModel> GetAllBatterySuitVehicle(GetAllBatterySuitVehicle getAllBatterySuitVehicle);
        Task<ResultModel> CreateBatteryByVehicleName(CreateBatteryByVehicleNameRequest createBatteryByVehicleNameRequest);
        Task<ResultModel> DeleteBatteryInStation(string batteryId);
        Task<ResultModel> AutoChargeAsync();
        Task<ResultModel> GetAllBatteriesPage(int pageNum, int pageSize) ;
    }
}
