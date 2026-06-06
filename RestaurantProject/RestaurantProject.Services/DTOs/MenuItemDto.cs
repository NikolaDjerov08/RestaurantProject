namespace Restaurant.Core.DTOs;

public class MenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
