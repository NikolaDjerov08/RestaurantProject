using Restaurant.Core.Enums;
using Restaurant.Core.Models;
using Restaurant.Core.Models.Categories;
using Restaurant.Core.Models.MenuItems;

namespace Restaurant.Web.Models.Menu;

public class MenuIndexViewModel
{
    public PagedResult<MenuItemServiceModel> Page { get; set; } = new();

    public IReadOnlyList<CategoryServiceModel> Categories { get; set; }
        = new List<CategoryServiceModel>();

    // Current filter state (so the form repopulates and paging links keep filters)
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public MenuSorting Sorting { get; set; } = MenuSorting.Newest;

    public IEnumerable<(MenuSorting Value, string Label)> SortOptions => new[]
    {
        (MenuSorting.Newest,    "Newest"),
        (MenuSorting.NameAsc,   "Name (A–Z)"),
        (MenuSorting.NameDesc,  "Name (Z–A)"),
        (MenuSorting.PriceAsc,  "Price (low → high)"),
        (MenuSorting.PriceDesc, "Price (high → low)")
    };
}
