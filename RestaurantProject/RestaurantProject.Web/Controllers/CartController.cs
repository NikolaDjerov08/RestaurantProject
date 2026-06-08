using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Contracts;

namespace Restaurant.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync();
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int menuItemId, int quantity = 1, string? returnUrl = null)
    {
        var added = await _cartService.AddAsync(menuItemId, quantity);

        if (added)
        {
            TempData["Success"] = "Added to your cart.";
        }
        else
        {
            TempData["Error"] = "That item is unavailable.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int menuItemId, int quantity)
    {
        await _cartService.UpdateQuantityAsync(menuItemId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int menuItemId)
    {
        await _cartService.RemoveAsync(menuItemId);
        TempData["Info"] = "Item removed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearAsync();
        TempData["Info"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));
    }
}
