using Contacts.Data;
using Contacts.Models;
using Contacts.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Contacts.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactRepository _repo;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,IContactRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {            
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
