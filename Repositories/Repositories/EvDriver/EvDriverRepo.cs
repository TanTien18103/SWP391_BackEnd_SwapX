using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EvDriver
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
    }
}
