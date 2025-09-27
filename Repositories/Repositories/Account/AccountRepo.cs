using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Account
{
    public class AccountRepo : IAccountRepo
    {
        private readonly SwapXContext _context;

        public AccountRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task AddAccount(BusinessObjects.Models.Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }



        public async Task<BusinessObjects.Models.Account> GetAccountByUserNameDao(string username)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
        }
        public async Task<BusinessObjects.Models.Account> GetAccountById(string accountId)
        {
           return await _context.Accounts
                .Include(a => a.BssStaffs).Include(a => a.Evdrivers)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
        public async Task<List<BusinessObjects.Models.Account>> GetAll()
        {
            return await _context.Accounts.Include(a => a.BssStaffs).Include(a => a.Evdrivers)
                .ToListAsync();
        }

        public async Task<BusinessObjects.Models.Account> UpdateAccount(BusinessObjects.Models.Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }
        public async Task<List<BusinessObjects.Models.Account>> GetAllStaff()
        {
            return await _context.Accounts.Include(a => a.BssStaffs).
                Where(a => a.Role == RoleEnums.Bsstaff.ToString()).ToListAsync();
        }
        public async Task<List<BusinessObjects.Models.Account>> GetAllCustomer()
        {
            return await _context.Accounts.Include(a => a.Evdrivers).
                Where(a => a.Role == RoleEnums.EvDriver.ToString()).ToListAsync();
        }
        

    }
}
