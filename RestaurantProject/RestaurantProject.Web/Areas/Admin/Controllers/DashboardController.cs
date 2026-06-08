using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;

namespace Restaurant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyConstants.StaffOnly)]
public class DashboardController : Controller
{
    private readonly IAdminService _adminService;

    public DashboardController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return View(stats);
    }
}
