using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Tests.Helpers;

public static class TestDbFactory
{
    /// <summary>
    /// Creates a fresh in-memory database with a unique name per call,
    /// so tests are fully isolated from each other.
    /// </summary>
    public static ApplicationDbContext CreateContext(string? name = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: name ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new ApplicationDbContext(options);
    }

    /// <summary>Seeds a small, predictable catalogue for service tests.</summary>
    public static async Task<ApplicationDbContext> CreateSeededContextAsync()
    {
        var context = CreateContext();

        var starters = new Category { Id = 1, Name = "Starters" };
        var mains = new Category { Id = 2, Name = "Main Course" };
        context.Categories.AddRange(starters, mains);

        context.MenuItems.AddRange(
            new MenuItem { Id = 1, Name = "Bruschetta", Description = "Tomato & basil toast", Price = 7.50m, CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 2, Name = "Caesar Salad", Description = "Romaine & parmesan", Price = 9.00m, CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 3, Name = "Ribeye Steak", Description = "Prime cut", Price = 32.00m, CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 4, Name = "Sold Out Dish", Description = "Currently unavailable", Price = 15.00m, CategoryId = 2, IsAvailable = false }
        );

        context.Users.Add(new ApplicationUser
        {
            Id = "user-1",
            UserName = "diner@test.com",
            Email = "diner@test.com",
            FullName = "Test Diner"
        });

        await context.SaveChangesAsync();
        return context;
    }
}
