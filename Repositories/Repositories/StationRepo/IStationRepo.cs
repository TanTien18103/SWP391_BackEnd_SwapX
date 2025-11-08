using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Repositories.StationRepo
{
    public interface IStationRepo
    {
        Task<Station> GetStationById(string stationId);
        Task<List<Station>> GetAllStations();
        Task AddStation(Station station);
        Task<Station> UpdateStation(Station station);
        Task<List<Station>> GetAllStationsOfCustomer();
        Task<List<Station>> GetAllStationsOfCustomerSuitVehicle(string vehicleId);
    }
}
