using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;

namespace LetranRPD.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    public IActionResult Account()
    {
        return View();
    }
    public IActionResult Certificates()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
