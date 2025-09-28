using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;

namespace LetranRPD.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Home");
    }
    public IActionResult Home()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Downloads()
    {
        return View();
    }

    public IActionResult Journals()
    {
        return View();
    }

    public IActionResult Services()
    {
        return View();
    }
    public IActionResult Avail_Services()
    {
        return View();
    }
    public IActionResult Tracker()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
