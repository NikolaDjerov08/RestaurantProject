namespace Restaurant.Core.Models.Users;

public class UserServiceModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ProfileImageUrl { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedOn { get; set; }
}
