using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.Report;
using Services.ApiModels;

namespace Services.Services.ReportService
{
    public interface IReportService
    {
        Task<ResultModel> GetReportById(string reportId);
        Task<ResultModel> GetAllReports();
        Task<ResultModel> AddReport(AddReportRequest addReportRequest);
        Task<ResultModel> UpdateReport(UpdateReportRequest updateReportRequest);
        Task<ResultModel> DeleteReport(string reportId);
    }
}
