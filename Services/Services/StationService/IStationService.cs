using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.ApiModels.Station;

namespace Services.Services.StationService
{
    public interface IStationService
    {
        Task<ResultModel> GetStationById(string stationId);
        Task<ResultModel> GetAllStations();
        Task<ResultModel> AddStation(AddStationRequest addStationRequest);
        Task<ResultModel> UpdateStation(UpdateStationRequest updateStationRequest);
        Task<ResultModel> DeleteStation(string stationId);
        Task<ResultModel> AddStaffToStation(AddStaffToStationRequest addStaffToStationRequest);
        Task<ResultModel> GetStaffsByStationId(string stationId);
        Task<ResultModel> RemoveStaffFromStation(string stationId, string staffId);
        Task<ResultModel> GetStationByStaffId(string staffId);
        Task<ResultModel> UpdateStationStatus(UpdateStationStatusRequest updateStationStatusRequest);
        Task<ResultModel> GetAllStationOfCustomer();
    }
}
