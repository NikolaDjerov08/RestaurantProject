using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Entities;
using Restaurant.Core.Models.Users;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserServiceModel?> GetProfileAsync(string userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserServiceModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Address = user.Address,
            ProfileImageUrl = user.ProfileImageUrl,
            CreatedOn = user.CreatedOn,
            Roles = roles
        };
    }

    public async Task<bool> UpdateProfileAsync(string userId, UserProfileFormModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return false;
        }

        user.FullName = model.FullName;
        user.Address = model.Address;
        user.ProfileImageUrl = model.ProfileImageUrl;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<UserServiceModel>> GetAllAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        var result = new List<UserServiceModel>(users.Count);
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            result.Add(new UserServiceModel
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                Address = u.Address,
                ProfileImageUrl = u.ProfileImageUrl,
                CreatedOn = u.CreatedOn,
                Roles = roles
            });
        }

        return result;
    }

    public async Task<bool> SoftDeleteAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return false;
        }

        user.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            return true;
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            return true;
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded;
    }
}
