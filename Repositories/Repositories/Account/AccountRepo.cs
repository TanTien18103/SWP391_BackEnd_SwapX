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
    }
}
