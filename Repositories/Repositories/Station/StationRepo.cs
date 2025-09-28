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
    public class StationRepo: IStationRepo
    {
        private readonly SwapXContext _context;
        public StationRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task AddStation(BusinessObjects.Models.Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
        }
        public async Task<List<BusinessObjects.Models.Station>> GetAllStations()
        {
            return await _context.Stations.ToListAsync();
        }

        public async Task<BusinessObjects.Models.Station> GetStationById(string stationId)
        {
              return await _context.Stations.FirstOrDefaultAsync(s => s.StationId == stationId);
        }

        public async Task<BusinessObjects.Models.Station> UpdateStation(BusinessObjects.Models.Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
            return station;
        }
    }
}
