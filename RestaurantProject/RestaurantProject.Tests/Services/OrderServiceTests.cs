using NUnit.Framework;
using Restaurant.Core.Enums;
using Restaurant.Core.Models.Cart;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Services;
using Restaurant.Tests.Helpers;

namespace Restaurant.Tests.Services;

[TestFixture]
public class OrderServiceTests
{
    private ApplicationDbContext _context = null!;
    private OrderService _service = null!;

    [SetUp]
    public async Task SetUp()
    {
        _context = await TestDbFactory.CreateSeededContextAsync();
        _service = new OrderService(_context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private static CartItemModel Line(int id, decimal price, int qty) =>
        new() { MenuItemId = id, Price = price, Quantity = qty, IsAvailable = true };

    [Test]
    public async Task CreateAsync_ComputesTotalFromDbPrices_NotCartPrices()
    {
        // Cart claims a fake low price; service must ignore it and use DB prices
        var cart = new[]
        {
            Line(1, price: 0.01m, qty: 2), // real price 7.50
            Line(3, price: 0.01m, qty: 1)  // real price 32.00
        };

        var result = await _service.CreateAsync("user-1", cart);

        Assert.That(result.Success, Is.True);
        var order = await _service.GetByIdAsync(result.OrderId);
        Assert.That(order, Is.Not.Null);
        // 7.50 * 2 + 32.00 = 47.00 — based on DB, not the 0.01 the cart claimed
        Assert.That(order!.TotalPrice, Is.EqualTo(47.00m));
    }

    [Test]
    public async Task CreateAsync_SnapshotsPriceOnEachOrderItem()
    {
        var cart = new[] { Line(2, price: 9.00m, qty: 3) };

        var result = await _service.CreateAsync("user-1", cart);
        var order = await _service.GetByIdAsync(result.OrderId);

        Assert.That(order!.Items, Has.Count.EqualTo(1));
        Assert.That(order.Items[0].Price, Is.EqualTo(9.00m));
        Assert.That(order.Items[0].Quantity, Is.EqualTo(3));
        Assert.That(order.Items[0].Subtotal, Is.EqualTo(27.00m));
    }

    [Test]
    public async Task CreateAsync_EmptyCart_Fails()
    {
        var result = await _service.CreateAsync("user-1", Array.Empty<CartItemModel>());

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Is.Not.Null);
    }

    [Test]
    public async Task CreateAsync_UnavailableItem_Fails()
    {
        var cart = new[] { Line(4, price: 15.00m, qty: 1) }; // item 4 is IsAvailable=false

        var result = await _service.CreateAsync("user-1", cart);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task CreateAsync_NonExistentItem_Fails()
    {
        var cart = new[] { Line(999, price: 5m, qty: 1) };

        var result = await _service.CreateAsync("user-1", cart);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task CreateAsync_NewOrder_StartsPending()
    {
        var result = await _service.CreateAsync("user-1", new[] { Line(1, 7.50m, 1) });
        var order = await _service.GetByIdAsync(result.OrderId);

        Assert.That(order!.Status, Is.EqualTo(OrderStatus.Pending));
    }

    [Test]
    public async Task UpdateStatusAsync_ChangesStatus()
    {
        var result = await _service.CreateAsync("user-1", new[] { Line(1, 7.50m, 1) });

        var updated = await _service.UpdateStatusAsync(result.OrderId, OrderStatus.Confirmed);

        Assert.That(updated, Is.True);
        var order = await _service.GetByIdAsync(result.OrderId);
        Assert.That(order!.Status, Is.EqualTo(OrderStatus.Confirmed));
    }

    [Test]
    public async Task UpdateStatusAsync_UnknownOrder_ReturnsFalse()
    {
        var updated = await _service.UpdateStatusAsync(123456, OrderStatus.Ready);
        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task GetByIdForUserAsync_WrongUser_ReturnsNull()
    {
        var result = await _service.CreateAsync("user-1", new[] { Line(1, 7.50m, 1) });

        var asOtherUser = await _service.GetByIdForUserAsync(result.OrderId, "someone-else");
        var asOwner = await _service.GetByIdForUserAsync(result.OrderId, "user-1");

        Assert.That(asOtherUser, Is.Null);
        Assert.That(asOwner, Is.Not.Null);
    }

    [Test]
    public async Task GetForUserAsync_ReturnsOnlyOwnOrders()
    {
        await _service.CreateAsync("user-1", new[] { Line(1, 7.50m, 1) });
        await _service.CreateAsync("user-1", new[] { Line(2, 9.00m, 1) });

        var orders = await _service.GetForUserAsync("user-1");

        Assert.That(orders, Has.Count.EqualTo(2));
        Assert.That(orders.All(o => o.UserId == "user-1"), Is.True);
    }
}
