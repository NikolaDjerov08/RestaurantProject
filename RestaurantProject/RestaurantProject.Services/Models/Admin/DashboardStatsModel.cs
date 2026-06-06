namespace Restaurant.Core.Models.Admin;

public class DashboardStatsModel
{
    public int TotalUsers { get; set; }
    public int TotalMenuItems { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int TotalReservations { get; set; }
    public int UpcomingReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenueLast30Days { get; set; }
    public List<TopMenuItemModel> TopSellingItems { get; set; } = new();
    public List<RevenueByMonthModel> RevenueByMonth { get; set; } = new();
}

public class TopMenuItemModel
{
    public int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}

public class RevenueByMonthModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Revenue { get; set; }
    public string Label => $"{new DateTime(Year, Month, 1):MMM yyyy}";
}
