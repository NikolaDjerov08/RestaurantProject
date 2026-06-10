using NUnit.Framework;
using Restaurant.Core.Entities;
using Restaurant.Core.Enums;
using Restaurant.Infrastructure.Services;
using Restaurant.Tests.Helpers;

namespace Restaurant.Tests.Services;

[TestFixture]
public class AdminServiceTests
{
    [Test]
    public async Task GetDashboardStatsAsync_EmptyDatabase_ReturnsAllZeros()
    {
        using var context = TestDbFactory.CreateContext();
        var service = new AdminService(context);

        var stats = await service.GetDashboardStatsAsync();

        Assert.That(stats.TotalUsers, Is.EqualTo(0));
        Assert.That(stats.TotalMenuItems, Is.EqualTo(0));
        Assert.That(stats.TotalCategories, Is.EqualTo(0));
        Assert.That(stats.TotalOrders, Is.EqualTo(0));
        Assert.That(stats.PendingOrders, Is.EqualTo(0));
        Assert.That(stats.TotalReservations, Is.EqualTo(0));
        Assert.That(stats.UpcomingReservations, Is.EqualTo(0));
        Assert.That(stats.TotalRevenue, Is.EqualTo(0m));
        Assert.That(stats.RevenueLast30Days, Is.EqualTo(0m));
        Assert.That(stats.TopSellingItems, Is.Empty);
        Assert.That(stats.RevenueByMonth, Is.Empty);
    }

    [Test]
    public async Task GetDashboardStatsAsync_WithData_ComputesCountsAndRevenue()
    {
        using var context = TestDbFactory.CreateContext();
        var now = DateTime.UtcNow;

        // Users: 2 active, 1 soft-deleted -> TotalUsers must be 2
        context.Users.AddRange(
            new ApplicationUser { Id = "u1", UserName = "a@t.com", Email = "a@t.com", FullName = "A" },
            new ApplicationUser { Id = "u2", UserName = "b@t.com", Email = "b@t.com", FullName = "B" },
            new ApplicationUser { Id = "u3", UserName = "c@t.com", Email = "c@t.com", FullName = "C", IsDeleted = true });

        context.Categories.AddRange(
            new Category { Id = 1, Name = "Starters" },
            new Category { Id = 2, Name = "Mains" });

        context.MenuItems.AddRange(
            new MenuItem { Id = 1, Name = "Bruschetta", Description = "x", Price = 10m, CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 2, Name = "Ribeye", Description = "x", Price = 32m, CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 3, Name = "Cake", Description = "x", Price = 8m, CategoryId = 2, IsAvailable = true });

        // Orders: 1 Delivered (recent), 1 Pending (recent), 1 Cancelled (excluded from revenue), 1 Confirmed (40 days ago)
        context.Orders.AddRange(
            new Order { Id = 1, UserId = "u1", Status = OrderStatus.Delivered, OrderDate = now.AddDays(-2), TotalPrice = 50m },
            new Order { Id = 2, UserId = "u2", Status = OrderStatus.Pending, OrderDate = now.AddDays(-10), TotalPrice = 30m },
            new Order { Id = 3, UserId = "u1", Status = OrderStatus.Cancelled, OrderDate = now.AddDays(-1), TotalPrice = 100m },
            new Order { Id = 4, UserId = "u2", Status = OrderStatus.Confirmed, OrderDate = now.AddDays(-40), TotalPrice = 20m });

        // Order items (only non-cancelled orders count toward top sellers)
        context.OrderItems.AddRange(
            new OrderItem { Id = 1, OrderId = 1, MenuItemId = 1, Quantity = 2, Price = 10m },
            new OrderItem { Id = 2, OrderId = 1, MenuItemId = 2, Quantity = 1, Price = 32m },
            new OrderItem { Id = 3, OrderId = 2, MenuItemId = 1, Quantity = 3, Price = 10m },
            new OrderItem { Id = 4, OrderId = 3, MenuItemId = 3, Quantity = 9, Price = 8m }); // cancelled -> ignored

        // Reservations: 1 upcoming, 1 past
        context.Reservations.AddRange(
            new Reservation { Id = 1, UserId = "u1", ReservationDate = now.AddDays(5), GuestsCount = 2 },
            new Reservation { Id = 2, UserId = "u2", ReservationDate = now.AddDays(-5), GuestsCount = 4 });

        await context.SaveChangesAsync();

        var service = new AdminService(context);
        var stats = await service.GetDashboardStatsAsync();

        Assert.That(stats.TotalUsers, Is.EqualTo(2));            // u3 is soft-deleted
        Assert.That(stats.TotalMenuItems, Is.EqualTo(3));
        Assert.That(stats.TotalCategories, Is.EqualTo(2));
        Assert.That(stats.TotalOrders, Is.EqualTo(4));
        Assert.That(stats.PendingOrders, Is.EqualTo(1));
        Assert.That(stats.TotalReservations, Is.EqualTo(2));
        Assert.That(stats.UpcomingReservations, Is.EqualTo(1));

        // Revenue excludes the cancelled order (100): 50 + 30 + 20 = 100
        Assert.That(stats.TotalRevenue, Is.EqualTo(100m));
        // Last 30 days excludes the 40-day-old order (20) and the cancelled one: 50 + 30 = 80
        Assert.That(stats.RevenueLast30Days, Is.EqualTo(80m));
    }

    [Test]
    public async Task GetDashboardStatsAsync_WithData_RanksTopSellingItems()
    {
        using var context = TestDbFactory.CreateContext();
        var now = DateTime.UtcNow;

        context.Users.Add(new ApplicationUser { Id = "u1", UserName = "a@t.com", Email = "a@t.com", FullName = "A" });
        context.Categories.Add(new Category { Id = 1, Name = "Mains" });
        context.MenuItems.AddRange(
            new MenuItem { Id = 1, Name = "Burger", Description = "x", Price = 10m, CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 2, Name = "Fries", Description = "x", Price = 4m, CategoryId = 1, IsAvailable = true });

        context.Orders.Add(new Order { Id = 1, UserId = "u1", Status = OrderStatus.Delivered, OrderDate = now.AddDays(-1), TotalPrice = 100m });
        context.OrderItems.AddRange(
            new OrderItem { Id = 1, OrderId = 1, MenuItemId = 1, Quantity = 5, Price = 10m },  // Burger: 5 units
            new OrderItem { Id = 2, OrderId = 1, MenuItemId = 2, Quantity = 2, Price = 4m });   // Fries: 2 units

        await context.SaveChangesAsync();

        var service = new AdminService(context);
        var stats = await service.GetDashboardStatsAsync();

        Assert.That(stats.TopSellingItems, Is.Not.Empty);
        Assert.That(stats.TopSellingItems.First().Name, Is.EqualTo("Burger"));
        Assert.That(stats.TopSellingItems.First().UnitsSold, Is.EqualTo(5));

        // Monthly revenue is grouped and should total the non-cancelled order revenue
        Assert.That(stats.RevenueByMonth.Sum(m => m.Revenue), Is.EqualTo(100m));
    }
}
