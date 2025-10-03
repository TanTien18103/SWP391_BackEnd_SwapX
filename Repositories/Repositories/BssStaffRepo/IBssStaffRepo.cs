using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BssStaffRepo
{
    public interface IBssStaffRepo
    {
        Task<BssStaff> GetBssStaffById(string staffId);
        Task<List<BssStaff>> GetAllBssStaffs();
        Task<BssStaff> AddBssStaff(BssStaff bssStaff);
        Task<BssStaff> UpdateBssStaff(BssStaff bssStaff);
        Task<BssStaff> GetBssStaffByAccountId(string accountId);
        Task<BssStaff> GetStationByStaffId(string staffId);
        Task<List<Account>> GetstaffsByStationId(string stationId);
    }
}
