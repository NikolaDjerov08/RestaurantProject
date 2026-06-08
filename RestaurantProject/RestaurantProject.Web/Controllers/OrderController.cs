using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Contracts;
using Restaurant.Web.Models.Cart;

namespace Restaurant.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public OrderController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetForUserAsync(GetUserId());
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetByIdForUserAsync(id, GetUserId());
        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = await _cartService.GetCartAsync();
        if (cart.IsEmpty)
        {
            TempData["Info"] = "Your cart is empty.";
            return RedirectToAction("Index", "Menu");
        }

        return View(new CheckoutViewModel { Cart = cart });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = await _cartService.GetCartAsync();
        if (cart.IsEmpty)
        {
            TempData["Info"] = "Your cart is empty.";
            return RedirectToAction("Index", "Menu");
        }

        // Re-bind cart for redisplay if validation fails
        model.Cart = cart;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _orderService.CreateAsync(GetUserId(), cart.Items);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Could not place order.");
            return View(model);
        }

        await _cartService.ClearAsync();
        TempData["Success"] = "Thank you — your order has been placed.";
        return RedirectToAction(nameof(Details), new { id = result.OrderId });
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim missing.");
}
