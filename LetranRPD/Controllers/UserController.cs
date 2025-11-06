using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LetranRPD.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ApplicationDBContext _context; // We need the database context

    // The constructor needs the ApplicationDBContext
    public UserController(ILogger<UserController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        _context = context; // Assign the context
    }

    public IActionResult Account()
    {
        return View();
    }

    // This is the new method for changing the password
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        // Get the current user from the session
        var currentUser = HttpContext.Session.GetObject<Account>("account");
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Error: You are not logged in." });
        }

        // Get a fresh instance of the user from the database
        var userFromDb = await _context.Accounts.FindAsync(currentUser.Id);
        if (userFromDb == null)
        {
            return Json(new { success = false, message = "Error: User not found." });
        }

        // --- 🚨 SECURITY WARNING ---
        // You are storing plain text passwords. This is very insecure.
        // In a real application, you must hash passwords.
        if (userFromDb.password != currentPassword)
        {
            return Json(new { success = false, message = "Incorrect current password." });
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            return Json(new { success = false, message = "New password must be at least 6 characters long." });
        }

        if (newPassword != confirmPassword)
        {
            return Json(new { success = false, message = "New passwords do not match." });
        }

        // --- 🚨 SECURITY WARNING ---
        // Again, you should be saving a HASH of the new password.
        userFromDb.password = newPassword;

        // Save the change to the database
        _context.Accounts.Update(userFromDb);
        await _context.SaveChangesAsync();

        // Update the session with the new user data
        HttpContext.Session.SetObject("account", userFromDb);

        return Json(new { success = true, message = "Password updated successfully!" });
    }
    // --- END OF NEW METHOD ---


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}