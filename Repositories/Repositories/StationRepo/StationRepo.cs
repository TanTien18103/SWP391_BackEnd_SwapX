using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.StationRepo
{
    public class StationRepo: IStationRepo
    {
        private readonly SwapXContext _context;
        public StationRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task AddStation(Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Station>> GetAllStations()
        {
            return await _context.Stations
                .Include(b=>b.Batteries)
                .Include(s => s.BssStaffs)
                .Include(s => s.Slots)
                .ToListAsync();
        }

        public async Task<Station> GetStationById(string stationId)
        {
            return await _context.Stations
                .Include(b=>b.Batteries)
                .Include(s => s.Slots)
                .FirstOrDefaultAsync(s => s.StationId == stationId);
        }

       

        public async Task<Station> UpdateStation(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
            return station;
        }
        public async Task<List<Station>> GetAllStationsOfCustomer()
        {

            return await _context.Stations
                .Where(s => s.Status == StationStatusEnum.Active.ToString())
                .Include(b => b.Batteries)
                .Include(s => s.BssStaffs)
                .Include(s => s.Slots)
                .ToListAsync();
        }
    }
}
