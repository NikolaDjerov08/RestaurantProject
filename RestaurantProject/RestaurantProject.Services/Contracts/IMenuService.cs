using Restaurant.Core.Models;
using Restaurant.Core.Models.MenuItems;

namespace Restaurant.Core.Contracts;

public interface IMenuService
{
    Task<PagedResult<MenuItemServiceModel>> QueryAsync(MenuQueryModel query);

    Task<MenuItemServiceModel?> GetByIdAsync(int id);

    Task<int> CreateAsync(MenuItemFormModel model);

    Task<bool> UpdateAsync(int id, MenuItemFormModel model);

    Task<bool> DeleteAsync(int id);

    Task<bool> ToggleAvailabilityAsync(int id);

    Task<bool> ExistsAsync(int id);
}
