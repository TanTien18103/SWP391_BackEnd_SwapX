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



        public async Task<BusinessObjects.Models.Account> GetAccountByUserName(string username)
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

        public async Task<BusinessObjects.Models.Account> GetAccountByEmail(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
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
    }
}
