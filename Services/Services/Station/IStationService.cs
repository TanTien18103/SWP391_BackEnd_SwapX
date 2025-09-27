using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.ApiModels.Station;

namespace Services.Services.Station
{
    public interface IStationService
    {
        Task<ResultModel> GetStationById(string stationId);
        Task<ResultModel> GetAllStations();
        Task<ResultModel> AddStation(AddStationRequest addStationRequest);
        Task<ResultModel> UpdateStation(UpdateStationRequest updateStationRequest);

    }
}
