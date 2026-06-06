using Restaurant.Core.Enums;

namespace Restaurant.Core.Models.Orders;

public class OrderServiceModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemServiceModel> Items { get; set; } = new();
}

public class OrderItemServiceModel
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal => Quantity * Price;
}
