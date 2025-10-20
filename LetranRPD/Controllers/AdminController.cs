using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;
using Microsoft.EntityFrameworkCore;


namespace LetranRPD.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDBContext context;

    public AdminController(ILogger<AdminController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }
    public IActionResult Research()
    {
        return View();
    }
    public IActionResult Create()
    {
        return View();
    }
    public IActionResult Submission()
    {
        return View();
    }


    public new IActionResult User()
    {
       
        return View();
    }
    public IActionResult Delete(string Name)
    {

        return RedirectToAction("User");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public new IActionResult Tracking()
    {
        var services = context.ServiceInformations.Include(o => o.ServiceProgress).OrderBy(i => i.ServiceId).ToList();
        return View(services);
    }

    public new IActionResult Certificates()
    {

        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
