using Restaurant.Core.Models.Users;

namespace Restaurant.Web.Models.Admin;

public class UserRolesViewModel
{
    public UserServiceModel User { get; set; } = new();
    public IReadOnlyList<string> AllRoles { get; set; } = new List<string>();
}
