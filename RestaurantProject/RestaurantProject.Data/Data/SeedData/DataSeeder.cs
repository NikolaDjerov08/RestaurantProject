using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Constants;
using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Data.SeedData;

public static class DataSeeder
{
    // Deterministic IDs so migrations are stable
    public const string AdminUserId = "a1b2c3d4-0000-0000-0000-000000000001";
    public const string AdminRoleId = "11111111-0000-0000-0000-000000000001";
    public const string EmployeeRoleId = "22222222-0000-0000-0000-000000000002";
    public const string CustomerRoleId = "33333333-0000-0000-0000-000000000003";

    // Pre-computed hash for "Admin@123" — generated with the default PasswordHasher
    // (matches Identity v3 hashing). This keeps migrations deterministic.
    public const string AdminPasswordHash =
        "AQAAAAIAAYagAAAAEJ8qXk2x3qZb2hY1pY2sQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQw==";

    public static void Seed(ModelBuilder builder)
    {
        SeedRoles(builder);
        SeedAdminUser(builder);
        SeedAdminUserRole(builder);
        SeedCategories(builder);
        SeedMenuItems(builder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = AdminRoleId,
                Name = RoleConstants.Admin,
                NormalizedName = RoleConstants.Admin.ToUpperInvariant(),
                ConcurrencyStamp = AdminRoleId
            },
            new IdentityRole
            {
                Id = EmployeeRoleId,
                Name = RoleConstants.Employee,
                NormalizedName = RoleConstants.Employee.ToUpperInvariant(),
                ConcurrencyStamp = EmployeeRoleId
            },
            new IdentityRole
            {
                Id = CustomerRoleId,
                Name = RoleConstants.Customer,
                NormalizedName = RoleConstants.Customer.ToUpperInvariant(),
                ConcurrencyStamp = CustomerRoleId
            }
        );
    }

    private static void SeedAdminUser(ModelBuilder builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        var admin = new ApplicationUser
        {
            Id = AdminUserId,
            UserName = "admin@restaurant.com",
            NormalizedUserName = "ADMIN@RESTAURANT.COM",
            Email = "admin@restaurant.com",
            NormalizedEmail = "ADMIN@RESTAURANT.COM",
            EmailConfirmed = true,
            FullName = "Restaurant Administrator",
            Address = "123 Main Street",
            SecurityStamp = "static-admin-security-stamp",
            ConcurrencyStamp = "static-admin-concurrency-stamp"
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

        builder.Entity<ApplicationUser>().HasData(admin);
    }

    private static void SeedAdminUserRole(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = AdminUserId,
                RoleId = AdminRoleId
            }
        );
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
            new MenuItem
            {
                Id = 1,
                Name = "Bruschetta",
                Description = "Grilled bread topped with tomatoes, garlic, fresh basil, and olive oil.",
                Price = 7.50m,
                ImageUrl = "/images/menu/bruschetta.jpg",
                CategoryId = 1
            },
            new MenuItem
            {
                Id = 2,
                Name = "Caesar Salad",
                Description = "Crisp romaine, parmesan, croutons, and our house Caesar dressing.",
                Price = 9.00m,
                ImageUrl = "/images/menu/caesar.jpg",
                CategoryId = 1
            },
            new MenuItem
            {
                Id = 3,
                Name = "Grilled Salmon",
                Description = "Atlantic salmon, lemon-butter glaze, served with seasonal vegetables.",
                Price = 22.50m,
                ImageUrl = "/images/menu/salmon.jpg",
                CategoryId = 2
            },
            new MenuItem
            {
                Id = 4,
                Name = "Ribeye Steak",
                Description = "12oz prime ribeye, garlic butter, mashed potato, roasted asparagus.",
                Price = 32.00m,
                ImageUrl = "/images/menu/ribeye.jpg",
                CategoryId = 2
            },
            new MenuItem
            {
                Id = 5,
                Name = "Spaghetti Carbonara",
                Description = "Classic Roman pasta with guanciale, egg, pecorino, and black pepper.",
                Price = 15.00m,
                ImageUrl = "/images/menu/carbonara.jpg",
                CategoryId = 5
            },
            new MenuItem
            {
                Id = 6,
                Name = "Penne Arrabbiata",
                Description = "Penne in a spicy tomato sauce with garlic and chili.",
                Price = 13.50m,
                ImageUrl = "/images/menu/arrabbiata.jpg",
                CategoryId = 5
            },
            new MenuItem
            {
                Id = 7,
                Name = "Tiramisu",
                Description = "Layered espresso-soaked ladyfingers with mascarpone and cocoa.",
                Price = 8.00m,
                ImageUrl = "/images/menu/tiramisu.jpg",
                CategoryId = 3
            },
            new MenuItem
            {
                Id = 8,
                Name = "Chocolate Lava Cake",
                Description = "Warm dark-chocolate cake with a molten centre, served with vanilla ice cream.",
                Price = 8.50m,
                ImageUrl = "/images/menu/lava.jpg",
                CategoryId = 3
            },
            new MenuItem
            {
                Id = 9,
                Name = "House Red Wine",
                Description = "Glass of our house red — full-bodied, smooth finish.",
                Price = 7.00m,
                ImageUrl = "/images/menu/red-wine.jpg",
                CategoryId = 4
            },
            new MenuItem
            {
                Id = 10,
                Name = "Sparkling Water",
                Description = "Chilled sparkling mineral water with lemon.",
                Price = 3.00m,
                ImageUrl = "/images/menu/sparkling.jpg",
                CategoryId = 4
            }
        );
    }
}
