using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.SlotRepo
{
    public interface ISlotRepo
    {
        Task<List<Slot>> GetAllSlots();
        Task<Slot> GetSlotById(string slotId);
        Task<Slot> AddSlot(Slot slot);
        Task<Slot> UpdateSlot(Slot slot);
        Task<List<Slot>> GetSlotsByStationId(string stationId);
        Task<List<Slot>> AddRange(List<Slot> slots);
        Task<Slot?> GetEmptySlotsByStationId(string stationId);
        Task<Slot> GetFirstAvailableSlot(string stationId);
        Task<Slot> GetByBatteryId(string batteryId);
    }
}
