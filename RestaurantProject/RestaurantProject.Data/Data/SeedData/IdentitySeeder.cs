using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Core.Constants;
using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Data.SeedData;

/// <summary>
/// Runtime identity seeding. Ensures the three roles and a default admin user
/// exist whenever the app starts. Idempotent — safe to run on every launch.
/// </summary>
public static class IdentitySeeder
{
    public const string AdminEmail = "admin@restaurant.com";
    public const string AdminPassword = "Admin@123";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { RoleConstants.Admin, RoleConstants.Employee, RoleConstants.Customer };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                FullName = "Restaurant Administrator",
                Address = "123 Main Street"
            };

            var result = await userManager.CreateAsync(admin, AdminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, RoleConstants.Admin))
        {
            await userManager.AddToRoleAsync(admin, RoleConstants.Admin);
        }
    }
}
