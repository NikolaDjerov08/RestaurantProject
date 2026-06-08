using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.Contracts;
using Restaurant.Core.Models.Reservations;

namespace Restaurant.Web.Controllers;

[Authorize]
public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var reservations = await _reservationService.GetForUserAsync(GetUserId());
        return View(reservations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ReservationFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationFormModel model)
    {
        // Server-side rule: no reservations in the past
        if (model.ReservationDate < DateTime.Now)
        {
            ModelState.AddModelError(nameof(model.ReservationDate),
                "Please choose a date and time in the future.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _reservationService.CreateAsync(GetUserId(), model);
        TempData["Success"] = "Your reservation request has been received.";
        return RedirectToAction(nameof(Index));
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim missing.");
}
