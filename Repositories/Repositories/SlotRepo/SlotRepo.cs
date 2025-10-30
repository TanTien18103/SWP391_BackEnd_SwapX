using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.SlotRepo
{
    public class SlotRepo : ISlotRepo
    {
        private readonly SwapXContext _context;
        public SlotRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task<List<Slot>> AddRange(List<Slot> slots)
        {
            _context.Slots.AddRange(slots);
            await _context.SaveChangesAsync();
            return slots;
        }

        public async Task<Slot> AddSlot(Slot slot)
        {
            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();
            return slot;
        }

        public async Task<List<Slot>> GetAllSlots()
        {
            return await _context.Slots.ToListAsync();
        }

        public async Task<Slot> GetByBatteryId(string batteryId)
        {
            return await _context.Slots.FirstOrDefaultAsync(s => s.BatteryId == batteryId);
        }

        public async Task<Slot?> GetEmptySlotsByStationId(string stationId)
        {
            return await _context.Slots
                .Where(s => s.StationId == stationId && s.Status == SlotStatusEnum.Empty.ToString())
                .OrderBy(s => s.CordinateY)
                .ThenBy(s => s.CordinateX)
                .FirstOrDefaultAsync();
        }

        public async Task<Slot> GetFirstAvailableSlot(string stationId)
        {
            return await _context.Slots
                .Where(s => s.StationId == stationId && s.Status == SlotStatusEnum.Empty.ToString())
                .OrderBy(s => s.CordinateY)
                .ThenBy(s => s.CordinateX)
                .FirstOrDefaultAsync();
        }

        public async Task<Slot> GetSlotById(string slotId)
        {
            return await _context.Slots.FirstOrDefaultAsync(s => s.SlotId == slotId);
        }

        public async Task<List<Slot>> GetSlotsByStationId(string stationId)
        {
            return await _context.Slots.Where(s => s.StationId == stationId).ToListAsync();
        }

        public Task<Slot> UpdateSlot(Slot slot)
        {
            _context.Slots.Update(slot);
            _context.SaveChangesAsync();
            return Task.FromResult(slot);
        }
    }
}
