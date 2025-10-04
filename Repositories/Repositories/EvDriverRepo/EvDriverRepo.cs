using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EvDriverRepo
{
    public class EvDriverRepo: IEvDriverRepo
    {
        private readonly SwapXContext _context;

        public EvDriverRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task AddDriver(Evdriver evdriver)
        {
            _context.Evdrivers.Add(evdriver);
            await _context.SaveChangesAsync();
        }



        public async Task<Evdriver> GetDriverByCustomerId(string customerId)
        {
            return await _context.Evdrivers.FirstOrDefaultAsync(b => b.CustomerId == customerId);
        }
        public async Task<Evdriver> GetDriverByAccountId(string accountId)
        {
            return await _context.Evdrivers.FirstOrDefaultAsync(e => e.AccountId == accountId);
        }
        public async Task<Evdriver> UpdateDriver(Evdriver evdriver)
        {
            _context.Evdrivers.Update(evdriver);
            await _context.SaveChangesAsync();
            return evdriver;
        }
    }
}
