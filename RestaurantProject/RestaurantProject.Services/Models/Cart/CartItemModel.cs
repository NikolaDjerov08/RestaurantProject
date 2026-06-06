namespace Restaurant.Core.Models.Cart;

public class CartItemModel
{
    public int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
    public decimal Subtotal => Price * Quantity;
}

public class CartSummaryModel
{
    public List<CartItemModel> Items { get; set; } = new();
    public int ItemCount => Items.Sum(i => i.Quantity);
    public decimal Total => Items.Sum(i => i.Subtotal);
    public bool IsEmpty => Items.Count == 0;
}
