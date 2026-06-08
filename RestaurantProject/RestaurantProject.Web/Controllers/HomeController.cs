using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About() => View();

    public IActionResult Contact() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCode(int code)
    {
        return View("StatusCode", new ErrorViewModel
        {
            StatusCode = code,
            Message = code switch
            {
                404 => "The page you're looking for doesn't exist.",
                403 => "You don't have permission to access this page.",
                500 => "Something went wrong on our end.",
                _ => "An error occurred."
            }
        });
    }
}
