using ContactsData.Models;
using Contacts.Repositories;
using ContactsData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Contacts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDapperContactRepository _dapperContactRepository; 
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
          IDapperContactRepository dapperContactRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _dapperContactRepository = dapperContactRepository;

            
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                //Questio: I know when we are going to call an async method we need await for the respone
                //         But if I forget to use awiat before calling the method, The error message is wierd and says contact doesn't have FirstName
                var contact = await _dapperContactRepository.GetByEmailAsync(User.Identity.Name);
                if (contact != null)
                {
                    ViewBag.Message = $"{contact.FirstName} Logged in Successfully";
                }

                
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
