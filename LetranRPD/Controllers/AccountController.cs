using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LetranRPD.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LetranRPD.Controllers;
using Microsoft.EntityFrameworkCore;

using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;

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

        private async Task _SendOtpEmailAsync(string toEmail, string otp)
        {
            string fromEmail = "charlesdominick.ocrisma@letran.edu.ph";
            string fromPassword = "lecjtwpxqigntdni"; 

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = "Your Registration OTP";
            message.To.Add(new MailAddress(toEmail));
            message.Body = $"<html><body>Your OTP code is: <strong>{otp}</strong></body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterButton(Account viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Email) ||
                !viewModel.Email.EndsWith("@letran.edu.ph", StringComparison.OrdinalIgnoreCase))
            {
                // Add an error specific to the Email field.
                ModelState.AddModelError("Email", "Registration is limited to Letran emails only");

                // Return the Register view so the user can fix the email.
                return View("Register", viewModel);
            }

            bool userExists = await dBContext.Accounts
                .AnyAsync(a => a.Email == viewModel.Email || a.StudentNumber == viewModel.StudentNumber);

            if (userExists)
            {
                ModelState.AddModelError(string.Empty, "An account with this email or student number already exists.");
                return View("Register", viewModel);
            }

            try
            {
                string otp = new Random().Next(100000, 999999).ToString();

                await _SendOtpEmailAsync(viewModel.Email, otp);


                TempData["PendingUser"] = JsonConvert.SerializeObject(viewModel);
                TempData["VerificationOtp"] = otp;

                return RedirectToAction("VerifyOtp");
            }
            catch (Exception ex)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("EMAIL SENDING FAILED:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("-----------------------------------");

                ModelState.AddModelError(string.Empty, "Could not send verification email. Please try again.");
                return View("Register", viewModel);
            }
        }

        // [GET] Action to show the OTP page
        // [GET] Action to show the OTP page
        public IActionResult VerifyOtp()
        {
            if (TempData["PendingUser"] == null)
            {
                return RedirectToAction("Register");
            }

            TempData.Keep("PendingUser");
            TempData.Keep("VerificationOtp");

            return View();
        }

        // [POST] Action to check the submitted OTP
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            // 1. Retrieve the stored data
            var pendingUserJson = TempData["PendingUser"] as string;
            var correctOtp = TempData["VerificationOtp"] as string;

            // 2. Check if data is missing (e.g., user waited too long)
            if (string.IsNullOrEmpty(pendingUserJson) || string.IsNullOrEmpty(correctOtp))
            {
                ModelState.AddModelError(string.Empty, "Your session has expired. Please register again.");
                return View();
            }

            // 3. Check if OTP is correct
            if (otp == correctOtp)
            {
                // --- SUCCESS ---
                // 4. Deserialize the user and save to database
                var account = JsonConvert.DeserializeObject<Account>(pendingUserJson);

                // This is where you would save the HASHED password
                await dBContext.Accounts.AddAsync(account);
                await dBContext.SaveChangesAsync();

                // Redirect to Login
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // --- FAILURE ---
                // 5. Show error and *re-set* the TempData so they can try again
                ModelState.AddModelError(string.Empty, "Invalid OTP. Please try again.");
                TempData["PendingUser"] = pendingUserJson; // Must re-add it
                TempData["VerificationOtp"] = correctOtp;   // Must re-add it
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> LoginButton(Account viewModel)
        {

            var userAccount = await dBContext.Accounts
                .FirstOrDefaultAsync(a => a.Email == viewModel.Email || a.StudentNumber == viewModel.Email);

            if (userAccount != null && userAccount.password == viewModel.password)
            {

                HttpContext.Session.SetObject("account", userAccount);
                Console.WriteLine("Match");


                return RedirectToAction("Index", "Home");
            }
            else
            {
                Console.WriteLine("Match'nt");

                ModelState.AddModelError(string.Empty, "Invalid student email or password.");

                return View("Login", viewModel);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("account");
            Console.WriteLine("Logout");
            return RedirectToAction("Index", "Home");


        }


        private async Task _SendPasswordResetEmailAsync(string toEmail, string otp)
        {
            string fromEmail = "charlesdominick.ocrisma@letran.edu.ph";
            string fromPassword = "lecjtwpxqigntdni"; // Your App Password

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = "Your Password Reset Code"; // Changed subject
            message.To.Add(new MailAddress(toEmail));
            message.Body = $"<html><body>Your password reset code is: <strong>{otp}</strong></body></html>"; // Changed body
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);
        }

        // --- UPDATED: This is your [HttpGet] ForgotPassword action ---
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            // Show the page in its initial state (State 1)
            ViewBag.IsCodeSent = false;
            return View();
        }

        // --- NEW: This action handles the "Send Code" button ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendResetCode(string email)
        {
            if (string.IsNullOrEmpty(email) ||
                !email.EndsWith("@letran.edu.ph", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid @letran.edu.ph email.");
                ViewBag.IsCodeSent = false; // Stay in State 1
                return View("ForgotPassword");
            }

            var userAccount = await dBContext.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (userAccount != null)
            {
                try
                {
                    string otp = new Random().Next(100000, 999999).ToString();
                    await _SendPasswordResetEmailAsync(userAccount.Email, otp);

                    TempData["ResetOTP"] = otp;
                    TempData["ResetEmail"] = userAccount.Email;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Password reset email failed: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Could not send reset email. Please try again.");
                    ViewBag.IsCodeSent = false;
                    return View("ForgotPassword");
                }
            }

            // In ALL cases, return the view in "Code Sent" state (State 2).
            ViewBag.IsCodeSent = true;
            ViewBag.Email = email; // Pass the email back to the view
            return View("ForgotPassword");
        }


        // --- NEW: This action handles the "Reset Password" button ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string otp, string newPassword, string confirmPassword)
        {
            ViewBag.IsCodeSent = true;
            ViewBag.Email = email;

            var correctOtp = TempData["ResetOTP"] as string;
            var correctEmail = TempData["ResetEmail"] as string;

            if (string.IsNullOrEmpty(correctOtp) || string.IsNullOrEmpty(correctEmail) || email != correctEmail)
            {
                ModelState.AddModelError(string.Empty, "Your password reset session has expired or is invalid. Please start over.");
                ViewBag.IsCodeSent = false; // Force user to start over
                return View("ForgotPassword");
            }

            if (otp != correctOtp)
            {
                ModelState.AddModelError(string.Empty, "Invalid code. Please try again.");
                TempData.Keep("ResetOTP");
                TempData.Keep("ResetEmail");
                return View("ForgotPassword");
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                TempData.Keep("ResetOTP");
                TempData.Keep("ResetEmail");
                return View("ForgotPassword");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            {
                ModelState.AddModelError(string.Empty, "Password must be at least 8 characters long.");
                TempData.Keep("ResetOTP");
                TempData.Keep("ResetEmail");
                return View("ForgotPassword");
            }

            var userAccount = await dBContext.Accounts.FirstOrDefaultAsync(a => a.Email == correctEmail);
            if (userAccount == null)
            {
                ModelState.AddModelError(string.Empty, "An error occurred. User not found.");
                return View("ForgotPassword");
            }

            // Save the new password (plain text, as requested)
            userAccount.password = newPassword;

            await dBContext.SaveChangesAsync();

            TempData["LoginMessage"] = "Your password has been reset. Please log in.";
            return RedirectToAction("Login");
        }


    }
}