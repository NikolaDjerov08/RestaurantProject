using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;
using Restaurant.Core.Models.Categories;

namespace Restaurant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyConstants.StaffOnly)]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create() => View(new CategoryFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormModel model)
    {
        if (await _categoryService.ExistsByNameAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), "A category with that name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _categoryService.CreateAsync(model);
        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        return View(new CategoryFormModel { Name = category.Name });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormModel model)
    {
        if (await _categoryService.ExistsByNameAsync(model.Name, excludeId: id))
        {
            ModelState.AddModelError(nameof(model.Name), "A category with that name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _categoryService.UpdateAsync(id, model);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Category updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        TempData[deleted ? "Success" : "Error"] = deleted
            ? "Category deleted."
            : "Can't delete a category that still has menu items.";
        return RedirectToAction(nameof(Index));
    }
}
