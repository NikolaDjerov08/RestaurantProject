using Restaurant.Core.Enums;

namespace Restaurant.Core.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
