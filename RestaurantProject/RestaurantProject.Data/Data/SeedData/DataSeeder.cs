using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Data.SeedData;

/// <summary>
/// Migration-time seed data for non-Identity reference data (categories, menu items).
/// Identity roles and the admin user are seeded at runtime by <see cref="IdentitySeeder"/>,
/// because Identity password hashes are non-deterministic and don't belong in HasData.
/// </summary>
public static class DataSeeder
{
    public static void Seed(ModelBuilder builder)
    {
        SeedCategories(builder);
        SeedMenuItems(builder);
    }

    private static void SeedCategories(ModelBuilder builder)
    {
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Starters" },
            new Category { Id = 2, Name = "Main Course" },
            new Category { Id = 3, Name = "Desserts" },
            new Category { Id = 4, Name = "Beverages" },
            new Category { Id = 5, Name = "Pasta" }
        );
    }

    private static void SeedMenuItems(ModelBuilder builder)
    {
        builder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "Bruschetta", Description = "Grilled bread topped with tomatoes, garlic, fresh basil, and olive oil.", Price = 7.50m, ImageUrl = "/images/menu/bruschetta.svg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 2, Name = "Caesar Salad", Description = "Crisp romaine, parmesan, croutons, and our house Caesar dressing.", Price = 9.00m, ImageUrl = "/images/menu/caesar.svg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 3, Name = "Grilled Salmon", Description = "Atlantic salmon, lemon-butter glaze, served with seasonal vegetables.", Price = 22.50m, ImageUrl = "/images/menu/salmon.svg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 4, Name = "Ribeye Steak", Description = "12oz prime ribeye, garlic butter, mashed potato, roasted asparagus.", Price = 32.00m, ImageUrl = "/images/menu/ribeye.svg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 5, Name = "Spaghetti Carbonara", Description = "Classic Roman pasta with guanciale, egg, pecorino, and black pepper.", Price = 15.00m, ImageUrl = "/images/menu/carbonara.svg", CategoryId = 5, IsAvailable = true },
            new MenuItem { Id = 6, Name = "Penne Arrabbiata", Description = "Penne in a spicy tomato sauce with garlic and chili.", Price = 13.50m, ImageUrl = "/images/menu/arrabbiata.svg", CategoryId = 5, IsAvailable = true },
            new MenuItem { Id = 7, Name = "Tiramisu", Description = "Layered espresso-soaked ladyfingers with mascarpone and cocoa.", Price = 8.00m, ImageUrl = "/images/menu/tiramisu.svg", CategoryId = 3, IsAvailable = true },
            new MenuItem { Id = 8, Name = "Chocolate Lava Cake", Description = "Warm dark-chocolate cake with a molten centre, served with vanilla ice cream.", Price = 8.50m, ImageUrl = "/images/menu/lava.svg", CategoryId = 3, IsAvailable = true },
            new MenuItem { Id = 9, Name = "House Red Wine", Description = "Glass of our house red, full-bodied with a smooth finish.", Price = 7.00m, ImageUrl = "/images/menu/red-wine.svg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 10, Name = "Sparkling Water", Description = "Chilled sparkling mineral water with lemon.", Price = 3.00m, ImageUrl = "/images/menu/sparkling.svg", CategoryId = 4, IsAvailable = true }
        );
    }
}
