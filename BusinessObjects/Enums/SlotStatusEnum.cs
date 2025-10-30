using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum SlotStatusEnum
    {
        /// <summary>
        /// Slot trống – không có pin bên trong.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Slot đang bị chiếm (pin đang được người dùng đổi vào, chưa hoàn tất).
        /// </summary>
        Occupied = 2,
    }
}
