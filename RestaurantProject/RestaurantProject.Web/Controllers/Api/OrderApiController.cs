using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;
using Restaurant.Core.DTOs;
using Restaurant.Core.Models.Orders;

namespace Restaurant.Web.Controllers.Api;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
[Authorize]
public class OrderApiController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderApiController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>GET /api/orders — current user's orders (or all, for staff).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> Get()
    {
        var orders = IsStaff()
            ? await _orderService.GetAllAsync()
            : await _orderService.GetForUserAsync(GetUserId());

        return Ok(orders.Select(MapToDto));
    }

    /// <summary>GET /api/orders/5 — own order, or any order for staff.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = IsStaff()
            ? await _orderService.GetByIdAsync(id)
            : await _orderService.GetByIdForUserAsync(id, GetUserId());

        if (order is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(order));
    }

    /// <summary>PUT /api/orders/5/status — staff only.</summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Policy = PolicyConstants.StaffOnly)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var updated = await _orderService.UpdateStatusAsync(id, dto.Status);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static OrderDto MapToDto(OrderServiceModel o) => new()
    {
        Id = o.Id,
        OrderDate = o.OrderDate,
        TotalPrice = o.TotalPrice,
        Status = o.Status.ToString(),
        Items = o.Items.Select(i => new OrderItemDto
        {
            MenuItemId = i.MenuItemId,
            MenuItemName = i.MenuItemName,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList()
    };

    private bool IsStaff() =>
        User.IsInRole(RoleConstants.Admin) || User.IsInRole(RoleConstants.Employee);

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim missing.");
}
