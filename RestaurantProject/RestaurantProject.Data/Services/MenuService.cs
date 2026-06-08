using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Entities;
using Restaurant.Core.Enums;
using Restaurant.Core.Models;
using Restaurant.Core.Models.MenuItems;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;

    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MenuItemServiceModel>> QueryAsync(MenuQueryModel query)
    {
        var menuQuery = _context.MenuItems
            .Include(m => m.Category)
            .AsNoTracking()
            .AsQueryable();

        if (!query.IncludeUnavailable)
        {
            menuQuery = menuQuery.Where(m => m.IsAvailable);
        }

        if (query.CategoryId is > 0)
        {
            menuQuery = menuQuery.Where(m => m.CategoryId == query.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLower();
            menuQuery = menuQuery.Where(m =>
                m.Name.ToLower().Contains(term) ||
                m.Description.ToLower().Contains(term));
        }

        menuQuery = query.Sorting switch
        {
            MenuSorting.NameAsc => menuQuery.OrderBy(m => m.Name),
            MenuSorting.NameDesc => menuQuery.OrderByDescending(m => m.Name),
            MenuSorting.PriceAsc => menuQuery.OrderBy(m => m.Price),
            MenuSorting.PriceDesc => menuQuery.OrderByDescending(m => m.Price),
            _ => menuQuery.OrderByDescending(m => m.Id)
        };

        var totalCount = await menuQuery.CountAsync();

        var pageSize = query.PageSize > 0 ? query.PageSize : MenuQueryModel.DefaultPageSize;
        var currentPage = query.CurrentPage < 1 ? 1 : query.CurrentPage;

        var items = await menuQuery
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MenuItemServiceModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                IsAvailable = m.IsAvailable,
                CategoryId = m.CategoryId,
                CategoryName = m.Category.Name
            })
            .ToListAsync();

        return new PagedResult<MenuItemServiceModel>
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = currentPage,
            PageSize = pageSize
        };
    }

    public async Task<MenuItemServiceModel?> GetByIdAsync(int id)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MenuItemServiceModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                IsAvailable = m.IsAvailable,
                CategoryId = m.CategoryId,
                CategoryName = m.Category.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(MenuItemFormModel model)
    {
        var entity = new MenuItem
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl,
            CategoryId = model.CategoryId,
            IsAvailable = model.IsAvailable
        };

        _context.MenuItems.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, MenuItemFormModel model)
    {
        var entity = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
        if (entity is null)
        {
            return false;
        }

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Price = model.Price;
        entity.ImageUrl = model.ImageUrl;
        entity.CategoryId = model.CategoryId;
        entity.IsAvailable = model.IsAvailable;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
        if (entity is null)
        {
            return false;
        }

        // Soft delete (query filter hides it from future queries)
        entity.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleAvailabilityAsync(int id)
    {
        var entity = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
        if (entity is null)
        {
            return false;
        }

        entity.IsAvailable = !entity.IsAvailable;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) =>
        _context.MenuItems.AnyAsync(m => m.Id == id);
}
