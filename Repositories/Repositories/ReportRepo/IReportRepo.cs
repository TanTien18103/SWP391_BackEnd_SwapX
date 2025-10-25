using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.ReportRepo
{
    public interface IReportRepo
    {
        Task<List<Report>> GetAllReports();
        Task<Report> GetReportById(string reportId);
        Task<Report> AddReport(Report report);
        Task<Report> UpdateReport(Report report);
        Task<List<Report>> GetReportsByStationId(string stationId);
    }
}
