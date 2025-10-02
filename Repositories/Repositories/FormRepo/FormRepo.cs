using BusinessObjects.Models;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.FormRepo
{
    public class FormRepo : IFormRepo
    {
        private readonly SwapXContext _context;
        public FormRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task<Form> Add(Form form)
        {
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<List<Form>> GetAll()
        {
            return await _context.Forms.ToListAsync();
        }

        public async Task<List<Form>> GetByAccountId(string accountId)
        {
            return await _context.Forms
                .Where(f => f.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<Form> GetById(string formId)
        {
            return await _context.Forms
                 .FirstOrDefaultAsync(f => f.FormId == formId);
        }

        public async Task<List<Form>> GetByStationId(string stationId)
        {
            return await _context.Forms
                .Where(f => f.StationId == stationId)
                .ToListAsync();
        }

        public async Task<Form> Update(Form form)
        {
            _context.Forms.Update(form);
            await _context.SaveChangesAsync();
            return form;
        }
        public async Task<List<Form>> GetFormsByAccountAndStation(string accountId, string stationId)
        {
            return await _context.Forms
                .Where(f => f.AccountId == accountId && f.StationId == stationId)
                .ToListAsync();
        }

    }
}
