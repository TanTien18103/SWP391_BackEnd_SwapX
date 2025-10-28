using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.StationScheduleRepo
{
    public interface IStationScheduleRepo
    {
        Task<StationSchedule> GetStationScheduleById(string stationScheduleId);
        Task<List<StationSchedule>> GetAllStationSchedules();
        Task<StationSchedule> AddStationSchedule(StationSchedule stationSchedule);
        Task<StationSchedule> UpdateStationSchedule(StationSchedule stationSchedule);
        Task<List<StationSchedule>> GetStationSchedulesByStationId(string stationId);
        Task<List<StationSchedule>> GetStationSchedulesByAccountId(string accountId);
    }
}
