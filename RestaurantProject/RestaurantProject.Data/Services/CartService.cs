using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Models.Cart;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

/// <summary>
/// Per-session cart. Stores only (MenuItemId, Quantity) tuples in HttpContext.Session;
/// hydrates with live menu data (name, image, current price, availability) on read.
/// </summary>
public class CartService : ICartService
{
    private const string SessionKey = "Restaurant.Cart";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public CartService(
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException(
                "Session is not available. Ensure UseSession() is configured in the pipeline.");

    public async Task<CartSummaryModel> GetCartAsync()
    {
        var stored = Load();
        if (stored.Count == 0)
        {
            return new CartSummaryModel();
        }

        var ids = stored.Select(s => s.MenuItemId).ToList();

        // Pull current menu data so we always show the live price/availability,
        // even if a cart item was added before an admin changed something.
        var menuItems = await _context.MenuItems
            .AsNoTracking()
            .Where(m => ids.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id);

        var summary = new CartSummaryModel();

        foreach (var s in stored)
        {
            if (!menuItems.TryGetValue(s.MenuItemId, out var m))
            {
                continue; // item deleted in the meantime; silently drop
            }

            summary.Items.Add(new CartItemModel
            {
                MenuItemId = m.Id,
                Name = m.Name,
                ImageUrl = m.ImageUrl,
                Price = m.Price,
                Quantity = s.Quantity,
                IsAvailable = m.IsAvailable
            });
        }

        return summary;
    }

    public async Task<bool> AddAsync(int menuItemId, int quantity = 1)
    {
        if (quantity < 1)
        {
            return false;
        }

        var exists = await _context.MenuItems
            .AsNoTracking()
            .AnyAsync(m => m.Id == menuItemId && m.IsAvailable);

        if (!exists)
        {
            return false;
        }

        var items = Load();
        var existing = items.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            items.Add(new StoredCartItem { MenuItemId = menuItemId, Quantity = quantity });
        }

        Save(items);
        return true;
    }

    public async Task<bool> UpdateQuantityAsync(int menuItemId, int quantity)
    {
        var items = Load();
        var existing = items.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (existing is null)
        {
            return false;
        }

        if (quantity < 1)
        {
            items.Remove(existing);
        }
        else
        {
            existing.Quantity = quantity;
        }

        Save(items);
        return await Task.FromResult(true);
    }

    public Task RemoveAsync(int menuItemId)
    {
        var items = Load();
        items.RemoveAll(i => i.MenuItemId == menuItemId);
        Save(items);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        Session.Remove(SessionKey);
        return Task.CompletedTask;
    }

    public Task<int> GetCountAsync()
    {
        var items = Load();
        return Task.FromResult(items.Sum(i => i.Quantity));
    }

    // --- session helpers

    private List<StoredCartItem> Load()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<StoredCartItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<StoredCartItem>>(json)
                   ?? new List<StoredCartItem>();
        }
        catch
        {
            return new List<StoredCartItem>();
        }
    }

    private void Save(List<StoredCartItem> items)
    {
        var json = JsonSerializer.Serialize(items);
        Session.SetString(SessionKey, json);
    }

    private sealed class StoredCartItem
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
