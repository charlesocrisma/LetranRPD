using LetranRPD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System; // Required for DateTime.Now
using System.Collections.Generic; // Required for List<ServiceInformation>

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace LetranRPD.Controllers
{

    public class ServiceForm : Controller
    {
        private readonly ApplicationDBContext dBContext;


        public List<ServiceInformation> ServiceInformationList = new();
        public ServiceForm(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public IActionResult Originality_Check()
        {
            return View();
        }
        public IActionResult Instrument_Validation()
        {
            return View();
        }
        public IActionResult Data_Analysis()
        {
            return View();
        }
        public IActionResult Language_Editing()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OriginalityButton(ServiceInformation viewModel)
        {

            var serviceInformation = new ServiceInformation
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
                OC_ManuscriptType = viewModel.OC_ManuscriptType,
                ServiceProgress = new ServiceProgress
                {
                    Progress1 = 1,
                    Progress2 = 0,
                    Progress3 = 0,
                    Progress4 = 0,
                    AppliedDate = DateTime.Now
                }


            };

            await dBContext.ServiceInformations.AddAsync(serviceInformation);

            await dBContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> InstrumentButton(ServiceInformation viewModel)
        {

            var serviceInformation = new ServiceInformation
            {
                StudentNumber = viewModel.StudentNumber,
                Email = viewModel.Email,
                ServiceType = "Instrument Validation",
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

            await dBContext.ServiceInformations.AddAsync(serviceInformation);

            await dBContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> LanguageButton(ServiceInformation viewModel)
        {

            var serviceInformation = new ServiceInformation
            {
                StudentNumber = viewModel.StudentNumber,
                Email = viewModel.Email,
                ServiceType = "Language Editing",
                Title = viewModel.Title,
                Author = viewModel.Author,
                ContactPerson = viewModel.ContactPerson,
                ContactNumber = viewModel.ContactNumber,
                ResearchAdviser = viewModel.ResearchAdviser,
                Subject = viewModel.Subject,
                LE_Index = viewModel.LE_Index,
                LE_Pages = viewModel.LE_Pages,
                ServiceProgress = new ServiceProgress
                {
                    Progress1 = 1,
                    Progress2 = 0,
                    Progress3 = 0,
                    Progress4 = 0,
                    AppliedDate = DateTime.Now
                }
            };

            await dBContext.ServiceInformations.AddAsync(serviceInformation);

            await dBContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> DataButton(ServiceInformation viewModel)
        {

            var serviceInformation = new ServiceInformation
            {
                StudentNumber = viewModel.StudentNumber,
                Email = viewModel.Email,
                ServiceType = "Data Analysis",
                Title = viewModel.Title,
                Author = viewModel.Author,
                ContactPerson = viewModel.ContactPerson,
                ContactNumber = viewModel.ContactNumber,
                ResearchAdviser = viewModel.ResearchAdviser,
                Subject = viewModel.Subject,
                DA_Tool = viewModel.DA_Tool,
                DA_Variable = viewModel.DA_Variable,
                ServiceProgress = new ServiceProgress
                {
                    Progress1 = 1,
                    Progress2 = 0,
                    Progress3 = 0,
                    Progress4 = 0,
                    AppliedDate = DateTime.Now
                }

            };

            await dBContext.ServiceInformations.AddAsync(serviceInformation);

            await dBContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }

    }
}
