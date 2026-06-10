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
            // ---- Starters (1) ----
            new MenuItem { Id = 1, Name = "Bruschetta", Description = "Grilled bread topped with tomatoes, garlic, fresh basil, and olive oil.", Price = 7.50m, ImageUrl = "/images/menu/bruschetta.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 2, Name = "Caesar Salad", Description = "Crisp romaine, parmesan, croutons, and our house Caesar dressing.", Price = 9.00m, ImageUrl = "/images/menu/caesar.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 11, Name = "Garlic Bread", Description = "Oven-baked baguette with garlic butter and a touch of parsley.", Price = 5.50m, ImageUrl = "/images/menu/garlic-bread.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 12, Name = "Mozzarella Sticks", Description = "Golden, crispy breaded mozzarella served with marinara dip.", Price = 6.50m, ImageUrl = "/images/menu/mozzarella-sticks.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 13, Name = "Chicken Wings", Description = "Crispy wings tossed in your choice of buffalo or BBQ sauce.", Price = 8.50m, ImageUrl = "/images/menu/chicken-wings.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 14, Name = "Tomato Soup", Description = "Creamy roasted-tomato soup with basil and a swirl of cream.", Price = 6.00m, ImageUrl = "/images/menu/tomato-soup.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 33, Name = "Onion Rings", Description = "Crispy golden battered onion rings served with a dipping sauce.", Price = 5.50m, ImageUrl = "/images/menu/onion-rings.jpg", CategoryId = 1, IsAvailable = true },
            new MenuItem { Id = 34, Name = "Fries", Description = "Thick-cut, golden, crispy fries with sea salt.", Price = 4.00m, ImageUrl = "/images/menu/fries.jpg", CategoryId = 1, IsAvailable = true },

            // ---- Main Course (2) ----
            new MenuItem { Id = 3, Name = "Grilled Salmon", Description = "Atlantic salmon, lemon-butter glaze, served with seasonal vegetables.", Price = 22.50m, ImageUrl = "/images/menu/salmon.jpg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 4, Name = "Ribeye Steak", Description = "12oz prime ribeye, garlic butter, mashed potato, roasted asparagus.", Price = 32.00m, ImageUrl = "/images/menu/ribeye.jpg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 15, Name = "Margherita Pizza", Description = "Wood-fired pizza with tomato, fresh mozzarella, and basil.", Price = 12.50m, ImageUrl = "/images/menu/margherita-pizza.jpg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 16, Name = "Cheeseburger", Description = "Beef patty, cheddar, lettuce, tomato, and house sauce with fries.", Price = 13.00m, ImageUrl = "/images/menu/cheeseburger.jpg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 17, Name = "Grilled Chicken", Description = "Herb-marinated chicken breast with grilled vegetables.", Price = 16.50m, ImageUrl = "/images/menu/grilled-chicken.jpg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 18, Name = "Fish and Chips", Description = "Beer-battered cod with thick-cut chips and tartar sauce.", Price = 14.50m, ImageUrl = "/images/menu/Fish_and_chips_blackpool.jpg", CategoryId = 2, IsAvailable = true },
            new MenuItem { Id = 19, Name = "BBQ Ribs", Description = "Slow-cooked pork ribs glazed in smoky barbecue sauce.", Price = 19.50m, ImageUrl = "/images/menu/bbq-ribs.jpg", CategoryId = 2, IsAvailable = true },

            // ---- Desserts (3) ----
            new MenuItem { Id = 7, Name = "Tiramisu", Description = "Layered espresso-soaked ladyfingers with mascarpone and cocoa.", Price = 8.00m, ImageUrl = "/images/menu/tiramisu.jpg", CategoryId = 3, IsAvailable = true },
            new MenuItem { Id = 8, Name = "Chocolate Cake", Description = "Rich, moist chocolate cake layered with silky chocolate ganache.", Price = 8.50m, ImageUrl = "/images/menu/chocolate-cake.jpg", CategoryId = 3, IsAvailable = true },
            new MenuItem { Id = 22, Name = "Cheesecake", Description = "Creamy New York-style cheesecake with a berry compote.", Price = 7.50m, ImageUrl = "/images/menu/cheesecake.jpg", CategoryId = 3, IsAvailable = true },
            new MenuItem { Id = 23, Name = "Apple Pie", Description = "Warm spiced apple pie with a flaky crust and vanilla ice cream.", Price = 6.50m, ImageUrl = "/images/menu/apple-pie.jpg", CategoryId = 3, IsAvailable = true },
            new MenuItem { Id = 24, Name = "Ice Cream Sundae", Description = "Vanilla ice cream, chocolate sauce, nuts, and whipped cream.", Price = 6.00m, ImageUrl = "/images/menu/ice-cream-sundae.jpg", CategoryId = 3, IsAvailable = true },

            // ---- Beverages (4) ----
            new MenuItem { Id = 9, Name = "House Red Wine", Description = "Glass of our house red, full-bodied with a smooth finish.", Price = 7.00m, ImageUrl = "/images/menu/red-wine.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 10, Name = "Water", Description = "Chilled bottled mineral water.", Price = 3.00m, ImageUrl = "/images/menu/water.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 25, Name = "Coca-Cola", Description = "Classic ice-cold cola served with lemon.", Price = 3.00m, ImageUrl = "/images/menu/coca-cola.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 26, Name = "Orange Juice", Description = "Freshly squeezed orange juice.", Price = 4.00m, ImageUrl = "/images/menu/orange-juice.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 27, Name = "Cappuccino", Description = "Espresso with steamed milk and a thick layer of foam.", Price = 4.50m, ImageUrl = "/images/menu/cappuccino.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 28, Name = "Lemonade", Description = "Homemade lemonade with fresh mint.", Price = 3.50m, ImageUrl = "/images/menu/lemonade.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 29, Name = "Craft Beer", Description = "Locally brewed craft lager, served chilled.", Price = 6.00m, ImageUrl = "/images/menu/craft-beer.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 30, Name = "Mojito", Description = "Rum, lime, mint, and soda over crushed ice.", Price = 8.50m, ImageUrl = "/images/menu/mojito.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 31, Name = "Espresso", Description = "Rich single-shot espresso.", Price = 3.00m, ImageUrl = "/images/menu/espresso.jpg", CategoryId = 4, IsAvailable = true },
            new MenuItem { Id = 32, Name = "Iced Tea", Description = "Refreshing iced tea with lemon.", Price = 3.50m, ImageUrl = "/images/menu/iced-tea.jpg", CategoryId = 4, IsAvailable = true },

            // ---- Pasta (5) ----
            new MenuItem { Id = 5, Name = "Spaghetti Carbonara", Description = "Classic Roman pasta with guanciale, egg, pecorino, and black pepper.", Price = 15.00m, ImageUrl = "/images/menu/carbonara.jpg", CategoryId = 5, IsAvailable = true },
            new MenuItem { Id = 6, Name = "Penne Arrabbiata", Description = "Penne in a spicy tomato sauce with garlic and chili.", Price = 13.50m, ImageUrl = "/images/menu/arrabbiata.jpg", CategoryId = 5, IsAvailable = true },
            new MenuItem { Id = 20, Name = "Lasagna", Description = "Layers of pasta, beef ragù, béchamel, and melted cheese.", Price = 14.00m, ImageUrl = "/images/menu/lasagna.jpg", CategoryId = 5, IsAvailable = true },
            new MenuItem { Id = 21, Name = "Fettuccine Alfredo", Description = "Fettuccine in a creamy parmesan and butter sauce.", Price = 13.00m, ImageUrl = "/images/menu/fettuccine-alfredo.jpg", CategoryId = 5, IsAvailable = true }
        );
    }
}
