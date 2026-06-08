namespace Restaurant.Core.DTOs;

public class CartDto
{
    public List<CartLineDto> Items { get; set; } = new();
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
}

public class CartLineDto
{
    public int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public bool IsAvailable { get; set; }
}

public class AddToCartDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartQuantityDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}
