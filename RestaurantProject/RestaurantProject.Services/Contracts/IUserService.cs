using Restaurant.Core.Models.Users;

namespace Restaurant.Core.Contracts;

public interface IUserService
{
    Task<UserServiceModel?> GetProfileAsync(string userId);

    Task<bool> UpdateProfileAsync(string userId, UserProfileFormModel model);

    Task<IReadOnlyList<UserServiceModel>> GetAllAsync();

    Task<bool> SoftDeleteAsync(string userId);

    Task<bool> AssignRoleAsync(string userId, string role);

    Task<bool> RemoveRoleAsync(string userId, string role);
}
