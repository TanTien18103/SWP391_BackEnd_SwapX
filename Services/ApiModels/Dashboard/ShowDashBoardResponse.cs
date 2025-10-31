using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Dashboard
{
    // KHẮC PHỤC LỖI CS0246: MonthlyData
    public class MonthlyData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalSwaps { get; set; }
    }

    // NEW: Model cho Phân tích giờ cao điểm
    public class HourSwapData
    {
        public int Hour { get; set; } // Giờ trong ngày (0 đến 23)
        public int TotalSwaps { get; set; } // Số lượt đổi pin trong giờ đó
        public int ActiveStations { get; set; } // Số trạm hoạt động trong giờ đó (Giả định đơn giản)
    }

    public class ShowDashboardResponse
    {
        // 1️⃣ Các số liệu tổng quan (Dashboard Summary)
        // ... (Giữ nguyên các thuộc tính Summary)
        public decimal CurrentMonthRevenue { get; set; }
        public decimal RevenueGrowthPercent { get; set; }
        public int CurrentMonthSwaps { get; set; }
        public decimal SwapsGrowthPercent { get; set; }
        public int TotalUsers { get; set; }
        public decimal NewUsersGrowthPercent { get; set; }
        public decimal AverageEfficiency { get; set; }
        public decimal EfficiencyGrowthPercent { get; set; }

        // 2️⃣ Dữ liệu cho biểu đồ Doanh thu & Lượt đổi pin theo tháng
        public List<MonthlyData> MonthlyRevenuesAndSwaps { get; set; }

        // 3️⃣ Dữ liệu cho biểu đồ Phân bố loại người dùng
        public List<UserDistributionData> UserDistribution { get; set; }

        // 4️⃣ Dữ liệu cho biểu đồ Tần suất đổi pin
        public List<DailySwapFrequencyData> DailySwapFrequency { get; set; }

        // 5️⃣ Dữ liệu cho biểu đồ Hiệu suất trạm đổi pin
        public List<StationEfficiencyData> StationEfficiencies { get; set; }

        // 6️⃣ NEW: Dữ liệu cho biểu đồ Phân tích giờ cao điểm
        public List<HourSwapData> HourlySwapAnalysis { get; set; }
    }

    // Model cho Phân bố người dùng
    public class UserDistributionData
    {
        public string UserType { get; set; }
        public int Count { get; set; }
        public decimal Percent { get; set; }
    }

    // Model cho Tần suất đổi pin theo ngày
    public class DailySwapFrequencyData
    {
        public DateTime Date { get; set; }
        public int TotalSwaps { get; set; }
        public decimal AvgTimeSpent { get; set; }
    }

    // Model cho Hiệu suất trạm đổi pin
    public class StationEfficiencyData
    {
        public string StationName { get; set; }
        public decimal EfficiencyPercent { get; set; }
        public int TotalSwaps { get; set; }
    }
}