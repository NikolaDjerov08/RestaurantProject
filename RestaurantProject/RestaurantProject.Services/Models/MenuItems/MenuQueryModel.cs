using Restaurant.Core.Enums;

namespace Restaurant.Core.Models.MenuItems;

public class MenuQueryModel
{
    public const int DefaultPageSize = 9;

    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public MenuSorting Sorting { get; set; } = MenuSorting.Newest;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;

    // Include unavailable items? Admin = true, customer = false.
    public bool IncludeUnavailable { get; set; } = false;
}
