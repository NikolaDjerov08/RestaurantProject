using Restaurant.Core.Models.Cart;

namespace Restaurant.Core.Contracts;

public interface ICartService
{
    Task<CartSummaryModel> GetCartAsync();

    Task<bool> AddAsync(int menuItemId, int quantity = 1);

    Task<bool> UpdateQuantityAsync(int menuItemId, int quantity);

    Task RemoveAsync(int menuItemId);

    Task ClearAsync();

    Task<int> GetCountAsync();
}
