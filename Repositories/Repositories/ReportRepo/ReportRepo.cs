using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.ReportRepo
{
    public class ReportRepo : IReportRepo
    {
        private readonly SwapXContext _context;
        public ReportRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<Report> AddReport(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }
        public async Task<List<Report>> GetAllReports()
        {
            return _context.Reports.ToList();
        }
        public async Task<Report> GetReportById(string reportId)
        {
            return _context.Reports.FirstOrDefault(r => r.ReportId == reportId);
        }
        public async Task<Report> UpdateReport(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
            return report;
        }
    }
}
