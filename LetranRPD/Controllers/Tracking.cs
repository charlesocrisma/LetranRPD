using LetranRPD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace LetranRPD.Controllers
{
    public class Tracking : Controller
    {
        private readonly ApplicationDBContext dBContext;

        public Tracking(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        [HttpPost]
        public async Task<IActionResult> add(ServiceInformation viewModel)
        {
            var ServiceInformation = new ServiceInformation
            {
                StudentNumber = viewModel.StudentNumber,
                Email = viewModel.Email,
                ServiceType = "Originality Check", // This was hardcoded in your original file
                Title = viewModel.Title,
                Author = viewModel.Author,
                ContactPerson = viewModel.ContactPerson,
                ContactNumber = viewModel.ContactNumber,
                ResearchAdviser = viewModel.ResearchAdviser,
                Subject = viewModel.Subject,
                ServiceProgress = new ServiceProgress
                {
                    Progress1 = 1, // 1 = Active
                    Progress2 = 0, // 0 = Pending
                    Progress3 = 0,
                    Progress4 = 0,
                    AppliedDate = DateTime.Now
                }
            };

            await dBContext.ServiceInformations.AddAsync(ServiceInformation);
            await dBContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }

        // This is the View Model for the new UpdateProgress action
        public class UpdateProgressViewModel
        {
            public int ServiceId { get; set; }
            public int StepNumber{ get;  set; }
            public int NewStatus { get; set; }
            public string Remarks { get; set; }
        }

        // This is the new action the JavaScript will call
        [HttpPost]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingService = await dBContext.ServiceInformations
                .Include(s => s.ServiceProgress)
                .FirstOrDefaultAsync(s => s.ServiceId == model.ServiceId);

            if (existingService == null)
            {
                return NotFound("Service not found.");
            }

            var progress = existingService.ServiceProgress;
            progress.Remarks = model.Remarks; // Update remarks

            // Update the correct progress step
            switch (model.StepNumber)
            {
                case 1:
                    progress.Progress1 = model.NewStatus;
                    break;
                case 2:
                    progress.Progress2 = model.NewStatus;
                    break;
                case 3:
                    progress.Progress3 = model.NewStatus;
                    break;
                case 4:
                    progress.Progress4 = model.NewStatus;
                    break;
                default:
                    return BadRequest("Invalid progress step.");
            }

            // --- Logic to automatically advance progress ---
            if (model.NewStatus == 2) // 2 = Completed
            {
                switch (model.StepNumber)
                {
                    case 1:
                        if (progress.Progress2 == 0) progress.Progress2 = 1; // 0 = Pending, 1 = Active
                        break;
                    case 2:
                        if (progress.Progress3 == 0) progress.Progress3 = 1;
                        break;
                    case 3:
                        if (progress.Progress4 == 0) progress.Progress4 = 1;
                        break;
                    case 4:
                        // This is the last step, no further advancement
                        break;
                }
            }

            await dBContext.SaveChangesAsync();

            // Return the updated progress state so the UI can sync
            return Json(new { success = true, message = "Progress updated.", updatedProgress = progress });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(int serviceId, int progressStep, List<IFormFile> files)
        {
            // --- This is the only change ---
            // If no files are sent, it's not an error. Just return OK.
            if (files == null || files.Count == 0)
                return Ok(new { success = true, message = "No files to upload." });
            // --- End of change ---

            var existingService = await dBContext.ServiceInformations
                .Include(s => s.ServiceProgress)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (existingService == null)
                return NotFound("Service not found.");

            var directory = Path.Combine("wwwroot", "uploads", "Service_" + serviceId);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), directory);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var safeFileName = Path.GetFileName(file.FileName);
                    var uniqueFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{safeFileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    savedFileNames.Add(uniqueFileName);
                }
            }

            var progress = existingService.ServiceProgress;

            switch (progressStep)
            {
                case 1:
                    progress.Progress1files ??= new List<string>();
                    progress.Progress1files.AddRange(savedFileNames);
                    break;
                case 2:
                    progress.Progress2files ??= new List<string>();
                    progress.Progress2files.AddRange(savedFileNames);
                    break;
                case 3:
                    progress.Progress3files ??= new List<string>();
                    progress.Progress3files.AddRange(savedFileNames);
                    break;
                case 4:
                    progress.Progress4files ??= new List<string>();
                    progress.Progress4files.AddRange(savedFileNames);
                    break;
                default:
                    return BadRequest("Invalid progress step.");
            }

            await dBContext.SaveChangesAsync();

            return Json(new { success = true, message = "Files uploaded successfully.", fileNames = savedFileNames });
        }
    }
}

