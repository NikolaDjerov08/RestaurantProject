using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Contracts;
using Restaurant.Core.DTOs;
using Restaurant.Core.Models.MenuItems;

namespace Restaurant.Web.Controllers.Api;

[ApiController]
[Route("api/menu")]
[Produces("application/json")]
public class MenuApiController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ICategoryService _categoryService;

    public MenuApiController(IMenuService menuService, ICategoryService categoryService)
    {
        _menuService = menuService;
        _categoryService = categoryService;
    }

    /// <summary>GET /api/menu?searchTerm=&categoryId=&sorting=&currentPage=&pageSize=</summary>
    [HttpGet]
    public async Task<ActionResult<object>> Get([FromQuery] MenuQueryModel query)
    {
        query.IncludeUnavailable = false; // public endpoint
        var page = await _menuService.QueryAsync(query);

        var items = page.Items.Select(MapToDto).ToList();

        return Ok(new
        {
            items,
            totalCount = page.TotalCount,
            currentPage = page.CurrentPage,
            pageSize = page.PageSize,
            totalPages = page.TotalPages,
            hasPrevious = page.HasPrevious,
            hasNext = page.HasNext
        });
    }

    /// <summary>GET /api/menu/5</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> GetById(int id)
    {
        var item = await _menuService.GetByIdAsync(id);
        if (item is null || !item.IsAvailable)
        {
            return NotFound();
        }

        return Ok(MapToDto(item));
    }

    /// <summary>GET /api/menu/categories</summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }));
    }

    private static MenuItemDto MapToDto(MenuItemServiceModel m) => new()
    {
        Id = m.Id,
        Name = m.Name,
        Description = m.Description,
        Price = m.Price,
        ImageUrl = m.ImageUrl,
        IsAvailable = m.IsAvailable,
        CategoryId = m.CategoryId,
        CategoryName = m.CategoryName
    };
}
