using Restaurant.Core.Models.Categories;

namespace Restaurant.Core.Contracts;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryServiceModel>> GetAllAsync();

    Task<CategoryServiceModel?> GetByIdAsync(int id);

    Task<int> CreateAsync(CategoryFormModel model);

    Task<bool> UpdateAsync(int id, CategoryFormModel model);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);

    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}
