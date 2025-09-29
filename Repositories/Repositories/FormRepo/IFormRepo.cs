using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.FormRepo
{
    public interface IFormRepo
    {
        Task<Form> Add(Form form);
        Task<Form> GetById(string formId);
        Task<List<Form>> GetAll();
        Task<List<Form>> GetByAccountId(string accountId);
        Task<List<Form>> GetByStationId(string stationId);
        Task<Form> Update(Form form);
    }
}
