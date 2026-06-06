namespace Restaurant.Core.Models.Categories;

public class CategoryServiceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MenuItemCount { get; set; }
}
