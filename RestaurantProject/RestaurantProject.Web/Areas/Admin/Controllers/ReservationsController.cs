using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;

namespace Restaurant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyConstants.StaffOnly)]
public class ReservationsController : Controller
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<IActionResult> Index()
    {
        var reservations = await _reservationService.GetAllAsync();
        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        var ok = await _reservationService.ConfirmAsync(id);
        TempData[ok ? "Success" : "Error"] = ok ? "Reservation confirmed." : "Reservation not found.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _reservationService.DeleteAsync(id);
        TempData[ok ? "Info" : "Error"] = ok ? "Reservation removed." : "Reservation not found.";
        return RedirectToAction(nameof(Index));
    }
}
