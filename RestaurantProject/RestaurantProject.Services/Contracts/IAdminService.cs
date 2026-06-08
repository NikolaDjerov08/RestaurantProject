using Restaurant.Core.Models.Admin;

namespace Restaurant.Core.Contracts;

public interface IAdminService
{
    Task<DashboardStatsModel> GetDashboardStatsAsync();
}
