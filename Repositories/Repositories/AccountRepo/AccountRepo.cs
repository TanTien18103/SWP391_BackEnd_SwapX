using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AccountRepo
{
    public class AccountRepo : IAccountRepo
    {
        private readonly SwapXContext _context;

        public AccountRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Account>> GetAccountsByUserName(string username)
        {
            return await _context.Accounts.Where(a => a.Username == username).ToListAsync();
        }

        public async Task<List<Account>> GetAccountsByEmail(string email)
        {
            return await _context.Accounts.Where(a => a.Email == email).ToListAsync();
        }

        public async Task<Account> GetAccountById(string accountId)
        {
            return await _context.Accounts
                 .Include(a => a.BssStaffs)
                 .Include(a => a.Evdrivers)
                 .ThenInclude(v => v.Vehicles)
                 .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
        public async Task<List<Account>> GetAll()
        {
            return await _context.Accounts.Include(a => a.BssStaffs).Include(a => a.Evdrivers)
                .ToListAsync();
        }

        public async Task<Account> UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }
        public async Task<List<Account>> GetAllStaff()
        {
            return await _context.Accounts.Include(a => a.BssStaffs).
                Where(a => a.Role == RoleEnums.Bsstaff.ToString()).ToListAsync();
        }
        public async Task<List<Account>> GetAllCustomer()
        {
            return await _context.Accounts.Include(a => a.Evdrivers).
                Where(a => a.Role == RoleEnums.EvDriver.ToString()).ToListAsync();
        }

        public async Task<string> GetAccountIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var accountIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                return accountIdClaim?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<Account> GetAccountByStaffId(string staffId)
        {
            var bssStaff = await _context.BssStaffs.FirstOrDefaultAsync(s => s.StaffId == staffId);
            if (bssStaff != null)
            {
                return await _context.Accounts
                    .Include(a => a.BssStaffs).Include(a => a.Evdrivers)
                    .FirstOrDefaultAsync(a => a.AccountId == bssStaff.AccountId);
            }
            return null;
        }

        public async Task<List<Account>> GetAllCustomerHasPackage()
        {
            return await _context.Accounts
                .Include(a => a.Evdrivers)
                .ThenInclude(e => e.Vehicles)
                .Where(a => a.Role == RoleEnums.EvDriver.ToString() && a.Evdrivers.Any(e => e.Vehicles.Any(v => v.PackageId != null)))
                .ToListAsync();
        }
    }
}
