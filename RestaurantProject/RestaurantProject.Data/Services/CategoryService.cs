using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Entities;
using Restaurant.Core.Models.Categories;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryServiceModel>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryServiceModel
            {
                Id = c.Id,
                Name = c.Name,
                MenuItemCount = c.MenuItems.Count(m => !m.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<CategoryServiceModel?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryServiceModel
            {
                Id = c.Id,
                Name = c.Name,
                MenuItemCount = c.MenuItems.Count(m => !m.IsDeleted)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CategoryFormModel model)
    {
        var entity = new Category { Name = model.Name };
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, CategoryFormModel model)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
        {
            return false;
        }

        entity.Name = model.Name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Categories
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity is null)
        {
            return false;
        }

        // Refuse delete if menu items still reference it
        var hasMenuItems = entity.MenuItems.Any(m => !m.IsDeleted);
        if (hasMenuItems)
        {
            return false;
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _context.Categories.AnyAsync(c => c.Id == id);

    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var normalized = name.Trim().ToLower();
        return _context.Categories.AnyAsync(c =>
            c.Name.ToLower() == normalized &&
            (excludeId == null || c.Id != excludeId));
    }
}
