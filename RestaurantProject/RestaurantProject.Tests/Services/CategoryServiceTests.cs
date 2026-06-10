using NUnit.Framework;
using Restaurant.Core.Models.Categories;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Services;
using Restaurant.Tests.Helpers;

namespace Restaurant.Tests.Services;

[TestFixture]
public class CategoryServiceTests
{
    private ApplicationDbContext _context = null!;
    private CategoryService _service = null!;

    [SetUp]
    public async Task SetUp()
    {
        _context = await TestDbFactory.CreateSeededContextAsync();
        _service = new CategoryService(_context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task GetAllAsync_ReturnsCategoriesWithItemCounts()
    {
        var categories = await _service.GetAllAsync();

        var starters = categories.FirstOrDefault(c => c.Name == "Starters");
        Assert.That(starters, Is.Not.Null);
        Assert.That(starters!.MenuItemCount, Is.EqualTo(2)); // Bruschetta + Caesar
    }

    [Test]
    public async Task CreateAsync_AddsCategory()
    {
        var id = await _service.CreateAsync(new CategoryFormModel { Name = "Desserts" });

        var category = await _service.GetByIdAsync(id);
        Assert.That(category!.Name, Is.EqualTo("Desserts"));
    }

    [Test]
    public async Task ExistsByNameAsync_DetectsDuplicates_CaseInsensitive()
    {
        var exists = await _service.ExistsByNameAsync("starters");
        Assert.That(exists, Is.True);
    }

    [Test]
    public async Task ExistsByNameAsync_ExcludeId_AllowsSameRecord()
    {
        // "Starters" is id 1 — excluding id 1 should report no conflict
        var conflict = await _service.ExistsByNameAsync("Starters", excludeId: 1);
        Assert.That(conflict, Is.False);
    }

    [Test]
    public async Task UpdateAsync_RenamesCategory()
    {
        var updated = await _service.UpdateAsync(1, new CategoryFormModel { Name = "Appetizers" });

        Assert.That(updated, Is.True);
        var category = await _service.GetByIdAsync(1);
        Assert.That(category!.Name, Is.EqualTo("Appetizers"));
    }

    [Test]
    public async Task DeleteAsync_WithMenuItems_IsRefused()
    {
        // Category 1 (Starters) has menu items
        var deleted = await _service.DeleteAsync(1);

        Assert.That(deleted, Is.False);
        Assert.That(await _service.ExistsAsync(1), Is.True);
    }

    [Test]
    public async Task DeleteAsync_EmptyCategory_Succeeds()
    {
        var emptyId = await _service.CreateAsync(new CategoryFormModel { Name = "Empty" });

        var deleted = await _service.DeleteAsync(emptyId);

        Assert.That(deleted, Is.True);
    }
}
