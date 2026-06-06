using Restaurant.Core.Enums;
using Restaurant.Core.Models.Cart;
using Restaurant.Core.Models.Orders;

namespace Restaurant.Core.Contracts;

public interface IOrderService
{
    /// <summary>Create an order from a hydrated cart. Recomputes the price server-side.</summary>
    Task<CreateOrderResult> CreateAsync(string userId, IReadOnlyCollection<CartItemModel> cartItems);

    Task<OrderServiceModel?> GetByIdAsync(int orderId);

    /// <summary>Return order for a user; null if not owned by that user.</summary>
    Task<OrderServiceModel?> GetByIdForUserAsync(int orderId, string userId);

    Task<IReadOnlyList<OrderServiceModel>> GetForUserAsync(string userId);

    Task<IReadOnlyList<OrderServiceModel>> GetAllAsync();

    Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus);
}
