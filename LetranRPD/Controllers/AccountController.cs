using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using LetranRPD.Controllers;
using Microsoft.EntityFrameworkCore;

namespace ResearchManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext dBContext;
         
        public List<Account> accountList = new();

        public AccountController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterButton(Account viewModel)
        {   

            var account = new Account
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                StudentNumber = viewModel.StudentNumber,
                Level = viewModel.Level,
                password = viewModel.password,
                isAdmin = viewModel.isAdmin

            };

            accountList = dBContext.Accounts.OrderByDescending(i => i.Id).ToList();

            foreach (var listMember in accountList)
            {
                if (listMember.StudentNumber == account.StudentNumber || listMember.Email == account.Email)
                {
                    Console.WriteLine("Registered already");
                    return RedirectToAction("Login", "Account");
                }
            }

            await dBContext.Accounts.AddAsync(account);

            await dBContext.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> LoginButton(Account viewModel)
        {
            accountList = dBContext.Accounts.OrderByDescending(i => i.Id).ToList();

            foreach (var account in accountList)
            {
                if ((account.Email == viewModel.Email || account.StudentNumber == viewModel.Email) && account.password == viewModel.password)
                {
                    HttpContext.Session.SetObject("account", account);
                    Console.WriteLine("Match");
                }
                else
                {
                    Console.WriteLine("Match'nt");
                }
            }



            return RedirectToAction("Index", "Home");

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("account");
            Console.WriteLine("Logout");
            return RedirectToAction("Index", "Home");


        }

        public IActionResult ForgotPassword()
        {
            return View();
        }


    }
}