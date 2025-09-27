using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Repositories.Station
{
    public interface IStationRepo
    {
        Task<BusinessObjects.Models.Station> GetStationById(string stationId);
        Task<List<BusinessObjects.Models.Station>> GetAllStations();
        Task AddStation(BusinessObjects.Models.Station station);
        Task<BusinessObjects.Models.Station> UpdateStation(BusinessObjects.Models.Station station);
    }
}
