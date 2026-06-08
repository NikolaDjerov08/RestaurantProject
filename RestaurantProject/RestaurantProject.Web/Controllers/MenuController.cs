using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Contracts;
using Restaurant.Core.Models.MenuItems;
using Restaurant.Web.Models.Menu;

namespace Restaurant.Web.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly ICategoryService _categoryService;

    public MenuController(IMenuService menuService, ICategoryService categoryService)
    {
        _menuService = menuService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] MenuQueryModel query)
    {
        // Customers only ever see available items
        query.IncludeUnavailable = false;

        var page = await _menuService.QueryAsync(query);
        var categories = await _categoryService.GetAllAsync();

        var vm = new MenuIndexViewModel
        {
            Page = page,
            Categories = categories,
            SearchTerm = query.SearchTerm,
            CategoryId = query.CategoryId,
            Sorting = query.Sorting
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var item = await _menuService.GetByIdAsync(id);
        if (item is null || !item.IsAvailable)
        {
            return NotFound();
        }

        return View(item);
    }
}
