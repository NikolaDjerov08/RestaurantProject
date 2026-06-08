using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.Core.Models.Categories;
using Restaurant.Core.Models.MenuItems;

namespace Restaurant.Web.Models.Admin;

public class MenuFormViewModel
{
    public int? Id { get; set; }
    public MenuItemFormModel Form { get; set; } = new();
    public IReadOnlyList<CategoryServiceModel> Categories { get; set; } = new List<CategoryServiceModel>();

    public IEnumerable<SelectListItem> CategoryOptions =>
        Categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = c.Id == Form.CategoryId
        });
}
