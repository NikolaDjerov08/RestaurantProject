using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;
using Restaurant.Web.Models.Admin;

namespace Restaurant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyConstants.AdminOnly)]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    private static readonly string[] AssignableRoles =
        { RoleConstants.Admin, RoleConstants.Employee, RoleConstants.Customer };

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Roles(string id)
    {
        var user = await _userService.GetProfileAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return View(new UserRolesViewModel
        {
            User = user,
            AllRoles = AssignableRoles
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string id, string role)
    {
        if (!AssignableRoles.Contains(role))
        {
            TempData["Error"] = "Unknown role.";
            return RedirectToAction(nameof(Roles), new { id });
        }

        await _userService.AssignRoleAsync(id, role);
        TempData["Success"] = $"Added {role} role.";
        return RedirectToAction(nameof(Roles), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(string id, string role)
    {
        // Guard: don't let an admin strip their own Admin role (lock-out protection)
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId && role == RoleConstants.Admin)
        {
            TempData["Error"] = "You can't remove your own Admin role.";
            return RedirectToAction(nameof(Roles), new { id });
        }

        await _userService.RemoveRoleAsync(id, role);
        TempData["Info"] = $"Removed {role} role.";
        return RedirectToAction(nameof(Roles), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId)
        {
            TempData["Error"] = "You can't delete your own account here.";
            return RedirectToAction(nameof(Index));
        }

        var ok = await _userService.SoftDeleteAsync(id);
        TempData[ok ? "Info" : "Error"] = ok ? "User deactivated." : "User not found.";
        return RedirectToAction(nameof(Index));
    }
}
