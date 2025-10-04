using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EvDriverRepo
{
    public interface IEvDriverRepo
    {
        Task AddDriver(Evdriver evdriver);
        Task<Evdriver> GetDriverByCustomerId(string customerId);
        Task<Evdriver> GetDriverByAccountId(string accountId);
        Task<Evdriver> UpdateDriver(Evdriver evdriver);
    }
}
