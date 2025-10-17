using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.ApiModels.StationSchedule;

namespace Services.Services.StationScheduleService
{
    public interface IStationScheduleService
    {
        Task<ResultModel> GetStationScheduleById(string stationScheduleId);
        Task<ResultModel> GetAllStationSchedules();
        Task<ResultModel> AddStationSchedule(AddStationScheduleRequest addStationScheduleRequest);
        Task<ResultModel> UpdateStationSchedule(UpdateStationScheduleRequest updateStationScheduleRequest);
        Task<ResultModel> DeleteStationSchedule(string stationScheduleId);
        Task<ResultModel> GetStationScheduleByStationId(string stationId);
        Task<ResultModel> UpdateStatusStationSchedule(UpdateStatusStationScheduleRequest updateStatusStationScheduleRequest);
    }
}
