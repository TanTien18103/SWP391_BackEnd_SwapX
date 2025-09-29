using BusinessObjects.Models;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Form
{
    public class FormRepo : IFormRepo
    {
        private readonly SwapXContext _context;
        public FormRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task<BusinessObjects.Models.Form> Add(BusinessObjects.Models.Form form)
        {
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<List<BusinessObjects.Models.Form>> GetAll()
        {
            return await _context.Forms.ToListAsync();
        }

        public async Task<List<BusinessObjects.Models.Form>> GetByAccountId(string accountId)
        {
            return await _context.Forms
                .Where(f => f.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<BusinessObjects.Models.Form> GetById(string formId)
        {
            return await _context.Forms
                 .FirstOrDefaultAsync(f => f.FormId == formId);
        }

        public async Task<List<BusinessObjects.Models.Form>> GetByStationId(string stationId)
        {
            return await _context.Forms
                .Where(f => f.StationId == stationId)
                .ToListAsync();
        }

        public async Task<BusinessObjects.Models.Form> Update(BusinessObjects.Models.Form form)
        {
            _context.Forms.Update(form);
            await _context.SaveChangesAsync();
            return form;
        }
    }
}
