using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;
using Restaurant.Core.Models.MenuItems;
using Restaurant.Web.Models.Admin;

namespace Restaurant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyConstants.StaffOnly)]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly ICategoryService _categoryService;

    public MenuController(IMenuService menuService, ICategoryService categoryService)
    {
        _menuService = menuService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index([FromQuery] MenuQueryModel query)
    {
        query.IncludeUnavailable = true; // admins see everything
        query.PageSize = 50;
        var page = await _menuService.QueryAsync(query);
        return View(page);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new MenuFormViewModel
        {
            Categories = await _categoryService.GetAllAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuFormViewModel vm)
    {
        if (!await _categoryService.ExistsAsync(vm.Form.CategoryId))
        {
            ModelState.AddModelError("Form.CategoryId", "Please select a valid category.");
        }

        if (!ModelState.IsValid)
        {
            vm.Categories = await _categoryService.GetAllAsync();
            return View(vm);
        }

        await _menuService.CreateAsync(vm.Form);
        TempData["Success"] = "Menu item created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _menuService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        var vm = new MenuFormViewModel
        {
            Id = item.Id,
            Form = new MenuItemFormModel
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                CategoryId = item.CategoryId,
                IsAvailable = item.IsAvailable
            },
            Categories = await _categoryService.GetAllAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MenuFormViewModel vm)
    {
        if (!await _categoryService.ExistsAsync(vm.Form.CategoryId))
        {
            ModelState.AddModelError("Form.CategoryId", "Please select a valid category.");
        }

        if (!ModelState.IsValid)
        {
            vm.Id = id;
            vm.Categories = await _categoryService.GetAllAsync();
            return View(vm);
        }

        var updated = await _menuService.UpdateAsync(id, vm.Form);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Menu item updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAvailability(int id)
    {
        await _menuService.ToggleAvailabilityAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _menuService.DeleteAsync(id);
        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Menu item deleted." : "Could not delete item.";
        return RedirectToAction(nameof(Index));
    }
}
