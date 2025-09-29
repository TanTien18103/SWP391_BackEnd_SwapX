using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Form
{
    public interface IFormRepo
    {
        Task<BusinessObjects.Models.Form> Add(BusinessObjects.Models.Form form);
        Task<BusinessObjects.Models.Form> GetById(string formId);
        Task<List<BusinessObjects.Models.Form>> GetAll();
        Task<List<BusinessObjects.Models.Form>> GetByAccountId(string accountId);
        Task<List<BusinessObjects.Models.Form>> GetByStationId(string stationId);
        Task<BusinessObjects.Models.Form> Update(BusinessObjects.Models.Form form);
    }
}
