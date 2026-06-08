using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Enums;
using Restaurant.Core.Models.Admin;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsModel> GetDashboardStatsAsync()
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);
        var sixMonthsAgo = now.AddMonths(-5);

        // Run independent counts in parallel-ish (sequential awaits are fine here;
        // EF Core connections aren't thread-safe for parallel queries on one context)
        var totalUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
        var totalMenuItems = await _context.MenuItems.CountAsync();
        var totalCategories = await _context.Categories.CountAsync();

        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders
            .CountAsync(o => o.Status == OrderStatus.Pending);

        var totalReservations = await _context.Reservations.CountAsync();
        var upcomingReservations = await _context.Reservations
            .CountAsync(r => r.ReservationDate >= now);

        var totalRevenue = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;

        var revenueLast30 = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled && o.OrderDate >= thirtyDaysAgo)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;

        var topItems = await _context.OrderItems
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .GroupBy(oi => new { oi.MenuItemId, oi.MenuItem.Name })
            .Select(g => new TopMenuItemModel
            {
                MenuItemId = g.Key.MenuItemId,
                Name = g.Key.Name,
                UnitsSold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Quantity * x.Price)
            })
            .OrderByDescending(x => x.UnitsSold)
            .Take(5)
            .ToListAsync();

        var monthly = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled && o.OrderDate >= sixMonthsAgo)
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(g => new RevenueByMonthModel
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(x => x.TotalPrice)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        return new DashboardStatsModel
        {
            TotalUsers = totalUsers,
            TotalMenuItems = totalMenuItems,
            TotalCategories = totalCategories,
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            TotalReservations = totalReservations,
            UpcomingReservations = upcomingReservations,
            TotalRevenue = totalRevenue,
            RevenueLast30Days = revenueLast30,
            TopSellingItems = topItems,
            RevenueByMonth = monthly
        };
    }
}
