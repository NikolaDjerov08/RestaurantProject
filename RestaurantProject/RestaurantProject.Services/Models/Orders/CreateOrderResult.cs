namespace Restaurant.Core.Models.Orders;

public class CreateOrderResult
{
    public bool Success { get; init; }
    public int OrderId { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreateOrderResult Ok(int orderId) =>
        new() { Success = true, OrderId = orderId };

    public static CreateOrderResult Fail(string message) =>
        new() { Success = false, ErrorMessage = message };
}
