using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;
using Restaurant.Core.Enums;

namespace Restaurant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyConstants.StaffOnly)]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var updated = await _orderService.UpdateStatusAsync(id, status);
        TempData[updated ? "Success" : "Error"] =
            updated ? $"Order #{id} marked as {status}." : "Order not found.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
