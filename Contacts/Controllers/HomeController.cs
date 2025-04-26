using Contacts.Data;
using Contacts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Contacts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext db,ILogger<HomeController> logger)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            List<Contact> contacts = _db.Contacts.Include(u=>u.Country).ToList();
            return View(contacts);
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
