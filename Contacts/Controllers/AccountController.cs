using Contacts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Contacts.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ILogger<AccountController> logger,IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;  // sending to view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl= null)
        {
            ViewData["ReturnUrl"] = returnUrl;  // do we need this line , we get the returnurl from query string

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User created a new account (Email: {user.Email}) with password.");

                    // Todo: Add email confirmation service
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail", "Account",
                        new { userId = user.Id, token = token },
                    protocol: HttpContext.Request.Scheme);

                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token },
                        protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");


                    //var hostPhone = "8187446619";

                    //
                    //DateTime now = DateTime.Now;
                    //string formattedDate = now.ToString("MMM d, yyyy");
                    //MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress("rgrekorian@gmail.com");
                    //mail.To.Add(@model.Email);
                    //mail.IsBodyHtml = true;
                    //mail.Subject = "Contact Registration";
                    //mail.Body =
                    //$"<h4><ul>Contact List</ul></h4>" +
                    //$"<h5> Dear {model.FirstName}, {model.LastName}</h4>" +
                    //$"<h5> Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.\"</h5>" +
                    //$"<h5>Date: {formattedDate}</h5>" +
                    //$"<h5><a href=\"tel:+1 {hostPhone}\" style=\"text-decoration:none;\">" +
                    //$"   <svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-telephone\" viewBox=\"0 0 16 16\">" +
                    //$"       <path d=\"M3.654 1.328a.678.678 0 0 0-1.015-.063L1.605 2.3c-.483.484-.661 1.169-.45 1.77a17.6 17.6 0 0 0 4.168 6.608 17.6 17.6 0 0 0 6.608 4.168c.601.211 1.286.033 1.77-.45l1.034-1.034a.678.678 0 0 0-.063-1.015l-2.307-1.794a.68.68 0 0 0-.58-.122l-2.19.547a1.75 1.75 0 0 1-1.657-.459L5.482 8.062a1.75 1.75 0 0 1-.46-1.657l.548-2.19a.68.68 0 0 0-.122-.58zM1.884.511a1.745 1.745 0 0 1 2.612.163L6.29 2.98c.329.423.445.974.315 1.494l-.547 2.19a.68.68 0 0 0 .178.643l2.457 2.457a.68.68 0 0 0 .644.178l2.189-.547a1.75 1.75 0 0 1 1.494.315l2.306 1.794c.829.645.905 1.87.163 2.611l-1.034 1.034c-.74.74-1.846 1.065-2.877.702a18.6 18.6 0 0 1-7.01-4.42 18.6 18.6 0 0 1-4.42-7.009c-.362-1.03-.037-2.137.703-2.877z\" />" +
                    //$"   </svg>" +
                    //$"    Phone : {hostPhone}" +
                    //$"</a></h5>" +
                    //$"<hr/>";

                    //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    //smtpClient.Port = 587;
                    //smtpClient.UseDefaultCredentials = false;
                    //smtpClient.Credentials = new NetworkCredential("rgrekorian@gmail.com", "deed zwmz hlyz thpl"); //rgrekorian@gmail.com  quickfix
                    //smtpClient.EnableSsl = true; // Enable SSL/TLS encryption
                    //smtpClient.Send(mail);

                    ViewBag.Message = "Verification Code sent to your Email";
                    TempData["Message"] = "Verification Code sent to your Email";
                    //HttpContext.Response.WriteAsync("<div id='notificationMessage' style='color:yellow'> User: " + "Your Repair Order sent successfully!" +" </div>");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login,string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password,login.RememberMe, false);
                if(result.Succeeded)
                {
                    _logger.LogInformation($"User Succesfully {login.Email} logged in");
                    ViewBag.Message = "Successfully Logged in";
                    TempData["Message"] = "Successfully Logged in";
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


    }
}
