using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LetranRPD.Controllers;

public class ProgressUpdateModel
{
    public int ServiceId { get; set; }
    public int Progress1 { get; set; }
    public int Progress2 { get; set; }
    public int Progress3 { get; set; }
    public int Progress4 { get; set; }
    public int RunCount { get; set; }
    public string? Remarks { get; set; }
}
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDBContext context; // Your DBContext variable


    public AdminController(ILogger<AdminController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        this.context = context; // Correctly uses 'context'
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


    public IActionResult User()
    {
        List<Account> allAccounts = context.Accounts.ToList();
        return View(allAccounts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserViewModel model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Invalid data." });
        }

        // Server-side validation
        if (await context.Accounts.AnyAsync(a => a.Email == model.email || a.StudentNumber == model.idno))
        {
            return Conflict(new { success = false, message = "Email or Student/Employee ID already exists." });
        }

        // Split "First Last" name into two parts
        string firstName = model.name;
        string lastName = "";
        var nameParts = model.name.Trim().Split(' ');
        if (nameParts.Length > 1)
        {
            firstName = nameParts[0];
            lastName = string.Join(" ", nameParts.Skip(1));
        }

        // Map ViewModel to the database Model
        var newAccount = new Account
        {
            FirstName = firstName,
            LastName = lastName,
            Email = model.email,
            StudentNumber = model.idno,
            Level = model.department, // Department
            isAdmin = model.role == "Admin" // Role
            // Password should be handled by a proper registration flow,
            // but for this form, we'll leave it as the default.
        };

        context.Accounts.Add(newAccount);
        await context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    // --- NEW METHOD for UPDATING Users ---
    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Invalid data." });
        }

        var userToUpdate = await context.Accounts.FindAsync(model.id);
        if (userToUpdate == null)
        {
            return NotFound(new { success = false, message = "User not found." });
        }

        // Server-side validation (check for duplicates *excluding* this user)
        if (await context.Accounts.AnyAsync(a => (a.Email == model.email || a.StudentNumber == model.idno) && a.Id != model.id))
        {
            return Conflict(new { success = false, message = "Email or Student/Employee ID already exists." });
        }

        // Split name
        string firstName = model.name;
        string lastName = "";
        var nameParts = model.name.Trim().Split(' ');
        if (nameParts.Length > 1)
        {
            firstName = nameParts[0];
            lastName = string.Join(" ", nameParts.Skip(1));
        }

        // Map updated data
        userToUpdate.FirstName = firstName;
        userToUpdate.LastName = lastName;
        userToUpdate.Email = model.email;
        userToUpdate.StudentNumber = model.idno;
        userToUpdate.Level = model.department;
        userToUpdate.isAdmin = model.role == "Admin";

        await context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    // --- NEW METHOD for DELETING Users ---
    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteViewModel model)
    {
        var userToDelete = await context.Accounts.FindAsync(model.id);
        if (userToDelete == null)
        {
            return NotFound(new { success = false, message = "User not found." });
        }

        context.Accounts.Remove(userToDelete);
        await context.SaveChangesAsync();

        return Ok(new { success = true });
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

        var serviceInfoList = context.ServiceInformations
                                     .Include(si => si.ServiceProgress)
                                     .OrderByDescending(si => si.ServiceProgress.AppliedDate) // Optional: sort by date
                                     .ToList();

        return View(serviceInfoList);
    }

    public new IActionResult Certificates()
    {

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"========== UPDATE PROGRESS START ==========");
        _logger.LogInformation($"ServiceId: {model.ServiceId}");
        _logger.LogInformation($"P1:{model.Progress1}, P2:{model.Progress2}, P3:{model.Progress3}, P4:{model.Progress4}");
        _logger.LogInformation($"RunCount: {model.RunCount}, Remarks: {model.Remarks}");

        try
        {
            // IMPORTANT: Use AsNoTracking first to verify the record exists
            var existingRecord = await context.ServiceProgresses
                .AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.ServiceId == model.ServiceId);

            if (existingRecord == null)
            {
                _logger.LogError($"ServiceProgress NOT FOUND for ServiceId: {model.ServiceId}");
                return NotFound(new { success = false, message = "Service progress record not found." });
            }

            _logger.LogInformation($"BEFORE UPDATE - Database values:");
            _logger.LogInformation($"  Id: {existingRecord.Id}");
            _logger.LogInformation($"  Progress1: {existingRecord.Progress1}");
            _logger.LogInformation($"  Progress2: {existingRecord.Progress2}");
            _logger.LogInformation($"  Progress3: {existingRecord.Progress3}");
            _logger.LogInformation($"  Progress4: {existingRecord.Progress4}");
            _logger.LogInformation($"  RunCount: {existingRecord.RunCount}");

            // Now find the tracked entity (or attach it)
            var serviceProgress = await context.ServiceProgresses
                .FirstOrDefaultAsync(sp => sp.ServiceId == model.ServiceId);

            if (serviceProgress == null)
            {
                // If for some reason it's not tracked, attach the existing record
                _logger.LogWarning("Entity not found in tracked context, attaching...");
                serviceProgress = existingRecord;
                context.ServiceProgresses.Attach(serviceProgress);
            }

            // Update the values
            serviceProgress.Progress1 = model.Progress1;
            serviceProgress.Progress2 = model.Progress2;
            serviceProgress.Progress3 = model.Progress3;
            serviceProgress.Progress4 = model.Progress4;
            serviceProgress.RunCount = model.RunCount;
            serviceProgress.Remarks = model.Remarks ?? "";

            _logger.LogInformation($"AFTER UPDATE - Memory values:");
            _logger.LogInformation($"  Progress1: {serviceProgress.Progress1}");
            _logger.LogInformation($"  Progress2: {serviceProgress.Progress2}");
            _logger.LogInformation($"  Progress3: {serviceProgress.Progress3}");
            _logger.LogInformation($"  Progress4: {serviceProgress.Progress4}");
            _logger.LogInformation($"  RunCount: {serviceProgress.RunCount}");

            // Force the entity state to Modified
            context.Entry(serviceProgress).State = EntityState.Modified;

            _logger.LogInformation($"Entity State: {context.Entry(serviceProgress).State}");

            // Check which properties are modified
            var modifiedProperties = context.Entry(serviceProgress)
                .Properties
                .Where(p => p.IsModified)
                .Select(p => p.Metadata.Name)
                .ToList();

            _logger.LogInformation($"Modified Properties: {string.Join(", ", modifiedProperties)}");

            // Save changes
            var changeCount = await context.SaveChangesAsync();

            _logger.LogInformation($"SaveChangesAsync returned: {changeCount} rows affected");

            // CRITICAL: Verify the change was actually written to database
            // Use a NEW context or clear change tracker
            context.ChangeTracker.Clear();

            var verifyRecord = await context.ServiceProgresses
                .AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.ServiceId == model.ServiceId);

            _logger.LogInformation($"VERIFICATION - Database values after save:");
            _logger.LogInformation($"  Progress1: {verifyRecord.Progress1}");
            _logger.LogInformation($"  Progress2: {verifyRecord.Progress2}");
            _logger.LogInformation($"  Progress3: {verifyRecord.Progress3}");
            _logger.LogInformation($"  Progress4: {verifyRecord.Progress4}");
            _logger.LogInformation($"  RunCount: {verifyRecord.RunCount}");

            bool actuallyChanged =
                verifyRecord.Progress1 == model.Progress1 &&
                verifyRecord.Progress2 == model.Progress2 &&
                verifyRecord.Progress3 == model.Progress3 &&
                verifyRecord.Progress4 == model.Progress4 &&
                verifyRecord.RunCount == model.RunCount;

            if (actuallyChanged)
            {
                _logger.LogInformation("✓✓✓ DATABASE WAS ACTUALLY UPDATED ✓✓✓");
            }
            else
            {
                _logger.LogError("✗✗✗ DATABASE WAS NOT UPDATED - VALUES DON'T MATCH ✗✗✗");
            }

            _logger.LogInformation($"========== UPDATE PROGRESS END ==========");

            return Ok(new
            {
                success = actuallyChanged,
                message = actuallyChanged
                    ? $"Successfully updated {changeCount} record(s)"
                    : "SaveChanges executed but database values didn't change!",
                data = new
                {
                    ServiceId = model.ServiceId,
                    Progress1 = verifyRecord.Progress1,
                    Progress2 = verifyRecord.Progress2,
                    Progress3 = verifyRecord.Progress3,
                    Progress4 = verifyRecord.Progress4,
                    RunCount = verifyRecord.RunCount,
                    Remarks = verifyRecord.Remarks
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR updating ServiceId: {ServiceId}", model.ServiceId);
            _logger.LogError($"Exception type: {ex.GetType().Name}");
            _logger.LogError($"Message: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                _logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }

            return StatusCode(500, new
            {
                success = false,
                message = $"Error: {ex.Message}",
                innerError = ex.InnerException?.Message
            });
        }
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}

public class UserViewModel
{
    public int id { get; set; } // Needed for update
    public string name { get; set; }
    public string email { get; set; }
    public string idno { get; set; }
    public string department { get; set; }
    public string role { get; set; }
}

// ViewModel to receive an ID for deletion
public class DeleteViewModel
{
    public int id { get; set; }
}