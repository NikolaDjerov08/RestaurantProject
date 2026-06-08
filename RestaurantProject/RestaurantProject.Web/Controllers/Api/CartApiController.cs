using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Contracts;
using Restaurant.Core.DTOs;
using Restaurant.Core.Models.Cart;

namespace Restaurant.Web.Controllers.Api;

[ApiController]
[Route("api/cart")]
[Produces("application/json")]
// State-changing actions require the antiforgery token (sent via X-CSRF-TOKEN header)
[AutoValidateAntiforgeryToken]
public class CartApiController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartApiController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> Get()
    {
        var cart = await _cartService.GetCartAsync();
        return Ok(MapToDto(cart));
    }

    [HttpGet("count")]
    public async Task<ActionResult<object>> Count()
    {
        var count = await _cartService.GetCountAsync();
        return Ok(new { count });
    }

    [HttpPost]
    public async Task<ActionResult<CartDto>> Add([FromBody] AddToCartDto dto)
    {
        var quantity = dto.Quantity < 1 ? 1 : dto.Quantity;
        var added = await _cartService.AddAsync(dto.MenuItemId, quantity);
        if (!added)
        {
            return BadRequest(new { message = "That item is unavailable." });
        }

        var cart = await _cartService.GetCartAsync();
        return Ok(MapToDto(cart));
    }

    [HttpPut]
    public async Task<ActionResult<CartDto>> Update([FromBody] UpdateCartQuantityDto dto)
    {
        await _cartService.UpdateQuantityAsync(dto.MenuItemId, dto.Quantity);
        var cart = await _cartService.GetCartAsync();
        return Ok(MapToDto(cart));
    }

    [HttpDelete("{menuItemId:int}")]
    public async Task<ActionResult<CartDto>> Remove(int menuItemId)
    {
        await _cartService.RemoveAsync(menuItemId);
        var cart = await _cartService.GetCartAsync();
        return Ok(MapToDto(cart));
    }

    [HttpDelete]
    public async Task<ActionResult<CartDto>> Clear()
    {
        await _cartService.ClearAsync();
        var cart = await _cartService.GetCartAsync();
        return Ok(MapToDto(cart));
    }

    private static CartDto MapToDto(CartSummaryModel cart) => new()
    {
        ItemCount = cart.ItemCount,
        Total = cart.Total,
        Items = cart.Items.Select(i => new CartLineDto
        {
            MenuItemId = i.MenuItemId,
            Name = i.Name,
            ImageUrl = i.ImageUrl,
            Price = i.Price,
            Quantity = i.Quantity,
            Subtotal = i.Subtotal,
            IsAvailable = i.IsAvailable
        }).ToList()
    };
}
