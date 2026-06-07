using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Entities;
using Restaurant.Core.Enums;
using Restaurant.Core.Models.Cart;
using Restaurant.Core.Models.Orders;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateOrderResult> CreateAsync(
        string userId,
        IReadOnlyCollection<CartItemModel> cartItems)
    {
        if (cartItems.Count == 0)
        {
            return CreateOrderResult.Fail("Cart is empty.");
        }

        // Pull current prices from DB — never trust the cart's claimed price
        var ids = cartItems.Select(c => c.MenuItemId).ToList();
        var dbItems = await _context.MenuItems
            .Where(m => ids.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id);

        var orderItems = new List<OrderItem>();
        decimal total = 0m;

        foreach (var cartItem in cartItems)
        {
            if (!dbItems.TryGetValue(cartItem.MenuItemId, out var menuItem))
            {
                return CreateOrderResult.Fail(
                    $"Menu item {cartItem.MenuItemId} no longer exists.");
            }

            if (!menuItem.IsAvailable)
            {
                return CreateOrderResult.Fail(
                    $"'{menuItem.Name}' is no longer available.");
            }

            if (cartItem.Quantity < 1)
            {
                return CreateOrderResult.Fail(
                    $"Invalid quantity for '{menuItem.Name}'.");
            }

            orderItems.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                Quantity = cartItem.Quantity,
                Price = menuItem.Price // snapshot at order time
            });

            total += menuItem.Price * cartItem.Quantity;
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalPrice = total,
            Status = OrderStatus.Pending,
            OrderItems = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreateOrderResult.Ok(order.Id);
    }

    public async Task<OrderServiceModel?> GetByIdAsync(int orderId)
    {
        return await BuildOrderQuery()
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<OrderServiceModel?> GetByIdForUserAsync(int orderId, string userId)
    {
        return await BuildOrderQuery()
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
    }

    public async Task<IReadOnlyList<OrderServiceModel>> GetForUserAsync(string userId)
    {
        return await BuildOrderQuery()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<OrderServiceModel>> GetAllAsync()
    {
        return await BuildOrderQuery()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null)
        {
            return false;
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    // Single source of truth for projection (DRY)
    private IQueryable<OrderServiceModel> BuildOrderQuery()
    {
        return _context.Orders
            .AsNoTracking()
            .Select(o => new OrderServiceModel
            {
                Id = o.Id,
                UserId = o.UserId,
                UserFullName = o.User.FullName,
                UserEmail = o.User.Email!,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemServiceModel
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            });
    }
}
