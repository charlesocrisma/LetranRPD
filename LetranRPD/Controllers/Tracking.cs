using LetranRPD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using LetranRPD.Controllers;
using Microsoft.EntityFrameworkCore;


namespace LetranRPD.Controllers

{
    public class Tracking : Controller
    {
        private readonly ApplicationDBContext dBContext;

        public List<ServiceInformation> ServiceInformationList = new();

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
                ServiceType = "Originality Check",
                Title = viewModel.Title,
                Author = viewModel.Author,
                ContactPerson = viewModel.ContactPerson,
                ContactNumber = viewModel.ContactNumber,
                ResearchAdviser = viewModel.ResearchAdviser,
                Subject = viewModel.Subject,
                ServiceProgress = new ServiceProgress
                {
                    Progress1 = 1,
                    Progress2 = 0,
                    Progress3 = 0,
                    Progress4 = 0,
                    AppliedDate = DateTime.Now
                }


            };

            await dBContext.ServiceInformations.AddAsync(ServiceInformation);

            await dBContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(ServiceInformation viewModel)
        //{
        //    // Find the existing record by ID (make sure viewModel.Id exists)
        //    var existingService = await dBContext.ServiceInformations
        //        .Include(s => s.ServiceProgress) // Include related progress data
        //        .FirstOrDefaultAsync(s => s.ServiceId == viewModel.ServiceId);

        //    if (existingService == null)
        //    {
        //        return NotFound(); // Return 404 if not found
        //    }

        //    // Update the properties


        //     //Update progress(optional)
        //    if (viewModel.ServiceProgress != null)
        //    {
        //        existingService.ServiceProgress.Progress1 = viewModel.ServiceProgress.Progress1;
        //        existingService.ServiceProgress.Progress2 = viewModel.ServiceProgress.Progress2;
        //        existingService.ServiceProgress.Progress3 = viewModel.ServiceProgress.Progress3;
        //        existingService.ServiceProgress.Progress4 = viewModel.ServiceProgress.Progress4;
        //        existingService.ServiceProgress.AppliedDate = viewModel.ServiceProgress.AppliedDate;
        //    }

        //    // Save changes
        //    await dBContext.SaveChangesAsync();

        //    // Redirect or return success
        //    return RedirectToAction("Services", "Home");
        //}

        [HttpPost]
        public async Task<IActionResult> UploadFiles(int serviceId, int progressStep, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            var existingService = await dBContext.ServiceInformations
                .Include(s => s.ServiceProgress)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (existingService == null)
                return NotFound("Service not found.");
            var directory =  "wwwroot/uploads/Service_"+serviceId;
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), directory);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                var uniqueFileName = $"{DateTime.Now.ToString("dd-MM-yyyy")}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                savedFileNames.Add(uniqueFileName);
            }

            // Update corresponding progress step list
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

            return Json(new { success = true, message = "Files uploaded and database updated successfully." });
        }


    }
}
