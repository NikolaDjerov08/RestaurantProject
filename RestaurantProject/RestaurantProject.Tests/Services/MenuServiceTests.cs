using NUnit.Framework;
using Restaurant.Core.Enums;
using Restaurant.Core.Models.MenuItems;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Services;
using Restaurant.Tests.Helpers;

namespace Restaurant.Tests.Services;

[TestFixture]
public class MenuServiceTests
{
    private ApplicationDbContext _context = null!;
    private MenuService _service = null!;

    [SetUp]
    public async Task SetUp()
    {
        _context = await TestDbFactory.CreateSeededContextAsync();
        _service = new MenuService(_context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task QueryAsync_ExcludesUnavailable_ByDefault()
    {
        var result = await _service.QueryAsync(new MenuQueryModel { IncludeUnavailable = false });

        Assert.That(result.Items.All(i => i.IsAvailable), Is.True);
        Assert.That(result.Items.Any(i => i.Name == "Sold Out Dish"), Is.False);
    }

    [Test]
    public async Task QueryAsync_IncludeUnavailable_ShowsAll()
    {
        var result = await _service.QueryAsync(new MenuQueryModel { IncludeUnavailable = true });

        Assert.That(result.Items.Any(i => i.Name == "Sold Out Dish"), Is.True);
    }

    [Test]
    public async Task QueryAsync_SearchTerm_MatchesNameCaseInsensitive()
    {
        var result = await _service.QueryAsync(new MenuQueryModel { SearchTerm = "bruschetta" });

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].Name, Is.EqualTo("Bruschetta"));
    }

    [Test]
    public async Task QueryAsync_SearchTerm_MatchesDescription()
    {
        var result = await _service.QueryAsync(new MenuQueryModel { SearchTerm = "parmesan" });

        Assert.That(result.Items.Any(i => i.Name == "Caesar Salad"), Is.True);
    }

    [Test]
    public async Task QueryAsync_CategoryFilter_ReturnsOnlyThatCategory()
    {
        var result = await _service.QueryAsync(new MenuQueryModel { CategoryId = 1 });

        Assert.That(result.Items.All(i => i.CategoryId == 1), Is.True);
        Assert.That(result.Items, Has.Count.EqualTo(2)); // Bruschetta + Caesar
    }

    [Test]
    public async Task QueryAsync_SortByPriceAsc_OrdersCorrectly()
    {
        var result = await _service.QueryAsync(new MenuQueryModel
        {
            Sorting = MenuSorting.PriceAsc,
            IncludeUnavailable = true
        });

        var prices = result.Items.Select(i => i.Price).ToList();
        Assert.That(prices, Is.Ordered.Ascending);
    }

    [Test]
    public async Task QueryAsync_SortByNameDesc_OrdersCorrectly()
    {
        var result = await _service.QueryAsync(new MenuQueryModel
        {
            Sorting = MenuSorting.NameDesc,
            IncludeUnavailable = true
        });

        var names = result.Items.Select(i => i.Name).ToList();
        Assert.That(names, Is.Ordered.Descending);
    }

    [Test]
    public async Task QueryAsync_Paging_RespectsPageSize()
    {
        var result = await _service.QueryAsync(new MenuQueryModel
        {
            PageSize = 2,
            CurrentPage = 1,
            IncludeUnavailable = true
        });

        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(4));
        Assert.That(result.TotalPages, Is.EqualTo(2));
        Assert.That(result.HasNext, Is.True);
        Assert.That(result.HasPrevious, Is.False);
    }

    [Test]
    public async Task QueryAsync_SecondPage_HasPreviousNotNext()
    {
        var result = await _service.QueryAsync(new MenuQueryModel
        {
            PageSize = 2,
            CurrentPage = 2,
            IncludeUnavailable = true
        });

        Assert.That(result.HasPrevious, Is.True);
        Assert.That(result.HasNext, Is.False);
    }

    [Test]
    public async Task CreateAsync_AddsItem()
    {
        var id = await _service.CreateAsync(new MenuItemFormModel
        {
            Name = "Tiramisu",
            Description = "Coffee dessert",
            Price = 8.00m,
            CategoryId = 1,
            IsAvailable = true
        });

        var item = await _service.GetByIdAsync(id);
        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Name, Is.EqualTo("Tiramisu"));
    }

    [Test]
    public async Task UpdateAsync_ChangesFields()
    {
        var updated = await _service.UpdateAsync(1, new MenuItemFormModel
        {
            Name = "Bruschetta Deluxe",
            Description = "Now with more basil",
            Price = 8.50m,
            CategoryId = 1,
            IsAvailable = true
        });

        Assert.That(updated, Is.True);
        var item = await _service.GetByIdAsync(1);
        Assert.That(item!.Name, Is.EqualTo("Bruschetta Deluxe"));
        Assert.That(item.Price, Is.EqualTo(8.50m));
    }

    [Test]
    public async Task UpdateAsync_UnknownId_ReturnsFalse()
    {
        var updated = await _service.UpdateAsync(999, new MenuItemFormModel
        {
            Name = "x", Description = "xxxxx", Price = 1m, CategoryId = 1
        });

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task DeleteAsync_SoftDeletes_HidesFromQueries()
    {
        var deleted = await _service.DeleteAsync(1);

        Assert.That(deleted, Is.True);
        var item = await _service.GetByIdAsync(1);
        Assert.That(item, Is.Null); // hidden by the soft-delete query filter
    }

    [Test]
    public async Task ToggleAvailabilityAsync_FlipsFlag()
    {
        var before = await _service.GetByIdAsync(1);
        await _service.ToggleAvailabilityAsync(1);
        var after = await _service.GetByIdAsync(1);

        Assert.That(after!.IsAvailable, Is.EqualTo(!before!.IsAvailable));
    }

    [Test]
    public async Task ExistsAsync_TrueForReal_FalseForMissing()
    {
        Assert.That(await _service.ExistsAsync(1), Is.True);
        Assert.That(await _service.ExistsAsync(999), Is.False);
    }
}
