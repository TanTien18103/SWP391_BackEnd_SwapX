using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EvDriver
{
    public interface IEvDriverRepo
    {
        Task AddDriver(BusinessObjects.Models.Evdriver evdriver);
    }
}
