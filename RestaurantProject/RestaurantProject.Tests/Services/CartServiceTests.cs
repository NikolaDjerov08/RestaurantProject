using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Services;
using Restaurant.Tests.Helpers;

namespace Restaurant.Tests.Services;

[TestFixture]
public class CartServiceTests
{
    private ApplicationDbContext _context = null!;
    private CartService _service = null!;
    private FakeSession _session = null!;

    [SetUp]
    public async Task SetUp()
    {
        _context = await TestDbFactory.CreateSeededContextAsync();
        _session = new FakeSession();

        // Moq: an HttpContext whose Session is our in-memory fake
        var httpContext = new DefaultHttpContext();
        typeof(DefaultHttpContext)
            .GetProperty(nameof(DefaultHttpContext.Session))!
            .SetValue(httpContext, _session);

        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(httpContext);

        _service = new CartService(accessor.Object, _context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task AddAsync_AvailableItem_AddsToCart()
    {
        var added = await _service.AddAsync(1, 2);

        Assert.That(added, Is.True);
        var cart = await _service.GetCartAsync();
        Assert.That(cart.ItemCount, Is.EqualTo(2));
        Assert.That(cart.Items[0].MenuItemId, Is.EqualTo(1));
    }

    [Test]
    public async Task AddAsync_UnavailableItem_Rejected()
    {
        var added = await _service.AddAsync(4, 1); // item 4 IsAvailable=false

        Assert.That(added, Is.False);
        var cart = await _service.GetCartAsync();
        Assert.That(cart.IsEmpty, Is.True);
    }

    [Test]
    public async Task AddAsync_SameItemTwice_IncrementsQuantity()
    {
        await _service.AddAsync(1, 1);
        await _service.AddAsync(1, 2);

        var cart = await _service.GetCartAsync();
        Assert.That(cart.Items, Has.Count.EqualTo(1));
        Assert.That(cart.Items[0].Quantity, Is.EqualTo(3));
    }

    [Test]
    public async Task GetCartAsync_HydratesWithCurrentDbPrice()
    {
        await _service.AddAsync(1, 1); // Bruschetta = 7.50

        var cart = await _service.GetCartAsync();

        Assert.That(cart.Items[0].Price, Is.EqualTo(7.50m));
        Assert.That(cart.Items[0].Name, Is.EqualTo("Bruschetta"));
    }

    [Test]
    public async Task GetCartAsync_ComputesTotalAcrossLines()
    {
        await _service.AddAsync(1, 2); // 7.50 * 2 = 15.00
        await _service.AddAsync(3, 1); // 32.00

        var cart = await _service.GetCartAsync();

        Assert.That(cart.Total, Is.EqualTo(47.00m));
    }

    [Test]
    public async Task UpdateQuantityAsync_ChangesQuantity()
    {
        await _service.AddAsync(1, 1);

        await _service.UpdateQuantityAsync(1, 5);

        var cart = await _service.GetCartAsync();
        Assert.That(cart.Items[0].Quantity, Is.EqualTo(5));
    }

    [Test]
    public async Task UpdateQuantityAsync_ToZero_RemovesLine()
    {
        await _service.AddAsync(1, 1);

        await _service.UpdateQuantityAsync(1, 0);

        var cart = await _service.GetCartAsync();
        Assert.That(cart.IsEmpty, Is.True);
    }

    [Test]
    public async Task RemoveAsync_RemovesItem()
    {
        await _service.AddAsync(1, 1);
        await _service.AddAsync(3, 1);

        await _service.RemoveAsync(1);

        var cart = await _service.GetCartAsync();
        Assert.That(cart.Items, Has.Count.EqualTo(1));
        Assert.That(cart.Items[0].MenuItemId, Is.EqualTo(3));
    }

    [Test]
    public async Task ClearAsync_EmptiesCart()
    {
        await _service.AddAsync(1, 1);
        await _service.AddAsync(3, 1);

        await _service.ClearAsync();

        var cart = await _service.GetCartAsync();
        Assert.That(cart.IsEmpty, Is.True);
    }

    [Test]
    public async Task GetCountAsync_SumsQuantities()
    {
        await _service.AddAsync(1, 2);
        await _service.AddAsync(3, 3);

        var count = await _service.GetCountAsync();

        Assert.That(count, Is.EqualTo(5));
    }

    [Test]
    public async Task AddAsync_ZeroOrNegativeQuantity_Rejected()
    {
        var added = await _service.AddAsync(1, 0);
        Assert.That(added, Is.False);
    }
}
