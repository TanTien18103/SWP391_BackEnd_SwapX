using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BssStaffRepo
{
    public class BssStaffRepo: IBssStaffRepo
    {
        private readonly SwapXContext _context;
        public BssStaffRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task<BssStaff> AddBssStaff(BssStaff bssStaff)
        {
            _context.BssStaffs.Add(bssStaff);
            await _context.SaveChangesAsync();
            return bssStaff;
        }

        public async Task<List<BssStaff>> GetAllBssStaffs()
        {
            return await _context.BssStaffs.ToListAsync();
        }

        public async Task<BssStaff> GetBssStaffByAccountId(string accountId)
        {
            return await _context.BssStaffs.FirstOrDefaultAsync(s => s.AccountId == accountId);
        }

        public async Task<BssStaff> GetBssStaffById(string staffId)
        {
            return await _context.BssStaffs.FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<List<Account>> GetstaffsByStationId(string stationId)
        {
            return await _context.BssStaffs
                .Include(s => s.Account)
                .Where(s => s.StationId == stationId)
                .Select(s => s.Account)
                .ToListAsync();
        }

        public async Task<BssStaff> GetStationByStaffId(string staffId)
        {
            return await _context.BssStaffs.Include(s => s.Station).FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<BssStaff> UpdateBssStaff(BssStaff bssStaff)
        {
            _context.BssStaffs.Update(bssStaff);
            await _context.SaveChangesAsync();
            return bssStaff;
        }
    }
}
