using Services.ApiModels;
using Services.ApiModels.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Form
{
    public interface IFormService
    {
        Task<ResultModel> AddForm(AddFormRequest addFormRequest);
        Task<ResultModel> GetFormById(string formId);
        Task<ResultModel> GetAllForms();
        Task<ResultModel> GetFormsByAccountId(string accountId);
        Task<ResultModel> GetFormsByStationId(string stationId);
        Task<ResultModel> UpdateForm(UpdateFormRequest updateFormRequest);
        Task<ResultModel> DeleteForm(string formId);
    }
}
