using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Dashboard
{
    // KHẮC PHỤC LỖI CS0246: MonthlyData
    public class ShowDashboardResponse
    {
        // Tổng quan hệ thống
        public int TotalStations { get; set; }              // Tổng số trạm
        public int TotalUsers { get; set; }                 // Tổng số người dùng
        public int TotalBatteries { get; set; }             // Tổng số pin
        public int TotalTransactions { get; set; }          // Tổng số giao dịch
        public decimal TotalRevenue { get; set; }           // Tổng doanh thu
        public int Totalorders { get; set; }

        // Dữ liệu thống kê theo thời gian
        public List<RevenueChartItem> RevenueOverTime { get; set; } = new();  // Doanh thu theo ngày/tháng
        public List<UserGrowthItem> UserGrowthChart { get; set; } = new();    // Tăng trưởng người dùng

        // Top 5 trạm có doanh thu cao nhất
        public List<TopStationItem> TopStations { get; set; } = new();

        // Doanh thu theo loại dịch vụ
        public List<RevenueByServiceTypeItem> RevenueByServiceType { get; set; } = new();

        // Thống kê trạng thái pin
        public BatteryStatusSummary BatteryStatusSummary { get; set; } = new();

        // Bộ lọc
        public DashboardFilterResponse FilterInfo { get; set; } = new();
    }
    public class RevenueChartItem
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class UserGrowthItem
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int TotalUsers { get; set; }
    }

    public class TopStationItem
    {
        public string StationId { get; set; }
        public string StationName { get; set; }
        public decimal Revenue { get; set; }
        public int TotalTransactions { get; set; }
        public int Totalorders { get; set; }
    }

    public class RevenueByServiceTypeItem
    {
        public int Orders { get; set; }
        public string ServiceType { get; set; }   // e.g. "Exchange", "Rent", "Recharge"
        public decimal Revenue { get; set; }
        public int Transactions { get; set; }
    }

    public class BatteryStatusSummary
    {
        public int Total { get; set; }
        public int Charging { get; set; }
        public int InUse { get; set; }
        public int Available { get; set; }
        public int Decommissioned { get; set; }
        public int Booked { get; set; }
        public int Maintenance { get; set; }

    }

    public class DashboardFilterResponse
    {
        public string SelectedStationId { get; set; }
        public string SelectedStationName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}